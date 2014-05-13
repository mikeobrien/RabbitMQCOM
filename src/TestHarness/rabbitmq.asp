<%
    Class PersonMessage
   
        Private m_name
        Private m_age
        Private m_birthday
 
        Public Property Let Name(value)
            m_name = value
        End Property
        Public Property Get Name()
            Name = m_name
        End Property
 
        Public Property Let Age(value)
            m_age = value
        End Property
        Public Property Get Age()
            Age = m_age
        End Property
 
        Public Property Let Birthday(value)
            m_birthday = value
        End Property
        Public Property Get Birthday()
            Birthday = m_birthday
        End Property
 
    End Class

    Set RabbitMQ = CreateObject("UltravioletCatastrophe.RabbitMQ")
    
    Set message = New PersonMessage 

    message.Name = "Rod"
    message.Age = 55
    message.Birthday = CDate("10/26/1985")

    RabbitMQ.Connect "amqp://localhost/"
    RabbitMQ.CreateDirectQueue "testexchange", "testroutingkey", "testqueue", false, false, false
    RabbitMQ.Publish "testexchange", "testroutingkey", false, message
    RabbitMQ.Disconnect

    Set RabbitMQ = Nothing
%>