<%

    Set RabbitMQ = CreateObject("UltravioletCatastrophe.RabbitMQ")
    
    Set xml = CreateObject("Msxml2.DOMDocument")
    xml.setProperty "SelectionLanguage", "XPath"
    xml.loadxml(BytesToStr(Request.BinaryRead(Request.TotalBytes)))

    RabbitMQ.Publish xml.selectSingleNode("//Body").nodeValue

    Set RabbitMQ = Nothing

    Function BytesToStr(bytes)
        Dim Stream
        Set Stream = Server.CreateObject("Adodb.Stream")
            Stream.Type = 1 'adTypeBinary
            Stream.Open
            Stream.Write bytes
            Stream.Position = 0
            Stream.Type = 2 'adTypeText
            Stream.Charset = "iso-8859-1"
            BytesToStr = Stream.ReadText
            Stream.Close
        Set Stream = Nothing
    End Function
%>