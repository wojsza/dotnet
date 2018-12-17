namespace ConfigurationTool.AppReporter.ComCommunication.Records

module UnionCase =
    type ComPortResponseTimeout = | Default | Value of int
