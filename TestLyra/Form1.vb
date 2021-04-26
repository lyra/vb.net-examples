Imports System.Threading

Imports System.IO
Imports System.Net
Imports System.Data
Imports System.Data.OleDb
Imports System.Text
Imports System.Xml
Imports System.Reflection
Imports System.Collections.ObjectModel
Imports Newtonsoft.Json

Public Class Form1

    Public Class ModelRequest
        Public Property amount As Integer
        Public Property currency As String
        Public Property orderId As String
    End Class

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        Dim requestUrl As String

        requestUrl = "https://api.lyra.com/api-payment/V4/Charge/CreatePaymentOrder"
        Dim params As New ModelRequest()
        params.amount = "33"
        params.currency = "EUR"
        params.orderId = "orderTest"

        Dim serializedResult = JsonConvert.SerializeObject(params)
        Dim responseContent As String = String.Empty

        Try
            Label1.Text = "Processing request ..."
            Dim responseCode = GetResponse(requestUrl, serializedResult, responseContent)
            Label1.Text = "Server answered"

            If responseCode = HttpStatusCode.OK Then
                Dim obj = JsonConvert.DeserializeObject(responseContent)
                If obj("status") = "SUCCESS" Then
                    Label1.Text = "URL is " + obj("answer")("paymentURL")
                Else
                    Label1.Text = "Error processing request (1): " + obj("answer")("errorCode") + ": " + obj("answer")("errorMessage")
                End If
                'Else
                '    Label1.Text = "Error processing request (2)"
            End If
        Catch ex As WebException
            Label1.Text = "Error processing request (3)" + ex.Message

        End Try
    End Sub

    Function GetResponse(ByVal requestUrl As String, ByVal json As String, ByRef responseContent As String) As HttpStatusCode

        ' .NET 4.0 does not support TLS 2, you should upgrade to the last version
        ServicePointManager.SecurityProtocol = 3072

        ' ADD YOUR OWN KEYS !!
        Dim authInfo = "69876357:testpassword_DEMOPRIVATEKEY23G4475zXZQ2UA5x7M"
        authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo))

        Dim request = WebRequest.Create(requestUrl)
        request.Method = "POST"
        request.Headers.Add("Authorization", "Basic " + authInfo)
        request.PreAuthenticate = True
        request.ContentType = "application/json; charset=utf-8"
        Dim jsonBytes = Encoding.UTF8.GetBytes(json)
        request.ContentLength = jsonBytes.Length
        Using reqStream = request.GetRequestStream()
            reqStream.Write(jsonBytes, 0, jsonBytes.Length)
        End Using

        Dim statusCode As HttpStatusCode

        Using response = DirectCast(request.GetResponse(), HttpWebResponse),
            responseStream = response.GetResponseStream(),
            reader = New StreamReader(responseStream)
            responseContent = reader.ReadToEnd()
            statusCode = response.StatusCode
        End Using
        Return statusCode
    End Function

End Class
