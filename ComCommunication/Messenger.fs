module Messenger

open System.IO.Ports
open System.Text

type SerialPort with

    member this.AsyncWriteLine(messange : string) = this.BaseStream.AsyncWrite(this.Encoding.GetBytes(messange + "\n"))

    member this.AsyncReadLine() =
        async {
            let messange_builder = StringBuilder()

            let buffer_ref = ref (Array.zeroCreate<byte> this.ReadBufferSize)
            let buffer = !buffer_ref
            let last_char = ref 0uy
      
            while !last_char <> byte '\n' do

                let! readCount = this.BaseStream.AsyncRead buffer
                last_char := buffer.[readCount-1]
                messange_builder.Append (this.Encoding.GetString(buffer.[0 .. readCount-1])) |> ignore

            messange_builder.Length <- messange_builder.Length-1 

            let response : string = messange_builder.ToString()

            printfn "Response: %s" response

            return response
        }

    member this.AsyncWriteLineAsByte(messange : byte[]) = this.BaseStream.AsyncWrite(messange)

    member this.AsyncReadLineAsByte() : Async<byte[]> =
        async { 

            let buffer_ref = ref (Array.zeroCreate<byte> this.BytesToRead)
            let buffer = !buffer_ref

            let! read_bytes = this.BaseStream.AsyncRead(buffer, 0, this.BytesToRead)

            return buffer
        }

type Data() =
    let mutable data : byte[] = null

    member this.Value
        with get() = data
        and set(value) = data <- value
