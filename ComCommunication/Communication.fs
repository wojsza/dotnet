namespace ConfigurationTool.AppReporter.ComCommunication

module Communication = 

    open Messenger
    open Records.UnionCase
    open Records.Configuration

    open System.IO
    open System
    open System.Threading
    open NLog.FSharp

    let time_now = System.DateTime.Now.ToShortTimeString()

    let ClosePort(port : Ports.SerialPort)  : Ports.SerialPort = 

        let close_port(port : Ports.SerialPort, date : string) = 
            port.Close()
            printfn "%s: Serial port already closed" date
                    
        match port.IsOpen with
            | true -> close_port(port, time_now)
            | false -> printfn "%s: Serial port already closed" time_now

        try
            port.Close()
        with
            | excp -> 
                let exception_msg : string = String.Format("Problem with disconnecting to port {0}, exception: {1}", port.PortName, excp.Message ) 
                printfn "%s" exception_msg
                raise (new UnauthorizedAccessException(exception_msg))
        port

    let OpenPort(com_port : string) : Ports.SerialPort  = 

        let port = new System.IO.Ports.SerialPort(com_port, 115200)

        try
            port.Open()
            port.DtrEnable <- true
            port.RtsEnable <- true
            port.ReadTimeout <- 10000

            match port.IsOpen with
                | true -> printfn "%s: Serial port %s opened" time_now com_port
                | false -> printfn "%s: Serial port %s can not be opened" time_now com_port

        with
            | excp -> 
                let exception_msg : string = String.Format("Connection to port {0} failed, exception: {1}", com_port, excp.Message) 
                printfn "%s" exception_msg  
                raise (new UnauthorizedAccessException(exception_msg))
        port

    
    let SendMessenge(port : Ports.SerialPort, messange: byte[]) : byte[]=
    
        let config = Settings()
        let timeout = GetTimeout(config, Value(0))
        
        let timeout_token = new CancellationTokenSource()
        timeout_token.CancelAfter(timeout)

        let combined_token =  CancellationTokenSource.CreateLinkedTokenSource (Async.DefaultCancellationToken, timeout_token.Token)

        async {
            let response = new Data()

            do! port.AsyncWriteLineAsByte messange  

            let mutable finished = false
            
            let! data_recived =
                async{
                    let! read_buffer = port.AsyncReadLineAsByte()
                    printfn "Response Buffer: %A" read_buffer
                    response.Value <- read_buffer
                    finished <- true
                }

            port.DataReceived.Add(fun _-> data_recived)

            while not finished do
                printfn "Wait for answer, Value is %A" response.Value

            // let! response = port.AsyncReadLineAsByte()
            return response.Value
        }
        |>  try
                fun response -> Async.RunSynchronously(response, timeout, combined_token.Token)
            with :? OperationCanceledException as e -> 
                if timeout_token.IsCancellationRequested then 
                    raise (new System.TimeoutException(e.Message))
                else 
                    reraise()


        