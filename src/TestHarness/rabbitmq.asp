﻿<%
    Set RabbitMQ = CreateObject("UltravioletCatastrophe.RabbitMQ")

    RabbitMQ.Connect "amqp://localhost/"
    RabbitMQ.CreateDirectQueue "testexchange", "testroutingkey", "testqueue", false, false, false

    message = "{""Name"":""Rod"",""Age"":55,""Birthday"":""10/26/1985""}"

    RabbitMQ.Publish "testexchange", "testroutingkey", false, message
    RabbitMQ.Disconnect

    Set RabbitMQ = Nothing
%>