namespace ConfigurationTool.AppReporter.ComCommunication.Records

module Configuration = 
    
    open FSharp.Configuration
    open System
    open UnionCase

    let[<Literal>] communincationAppSettingsPath : string = "communication.yaml"   
    type Settings = YamlConfig<communincationAppSettingsPath>

    let GetTimeout(config: Settings, timeout_type: ComPortResponseTimeout) : int = 

        let timeout_from_config = float config.SerialPortCommunication.Timeout;
        let timeout_miliseconds : int = Convert.ToInt32(TimeSpan.FromSeconds(timeout_from_config).TotalMilliseconds)

        let timeout : int =  match timeout_type with | Default -> 5000 | Value(value) -> timeout_miliseconds
        timeout   
