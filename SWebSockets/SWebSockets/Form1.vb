Imports System.Net.WebSockets
Imports System.Text
Imports System.Threading
Imports System.Text.Json

Public Class Form1

    'クライアント側のWebSocketを定義
    Dim ws As ClientWebSocket = New ClientWebSocket()

    '接続先エンドポイントを指定
    Dim uri = New Uri("ws://localhost:53863/ResponseHandler.ashx")
    'webHandler.ashx
    'ResponseHandler.ashx
    ''' <summary>
    ''' 接続ボタンクリック
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Connect()
    End Sub

    Private Async Sub Connect()
        ' サーバに対し、接続を開始
        Await ws.ConnectAsync(uri, CancellationToken.None)

    End Sub
    ''' <summary>
    ''' 受信開始ボタンクリック
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Start()
    End Sub

    Private Sub Start()

        'While (True)

        Receive()

        'End While

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click

        'Dim sendchara As String = "テストですよ"

        Dim sendchara As String = TextBox1.Text

        Dim buffer() As Byte = Encoding.UTF8.GetBytes(sendchara)

        '送信情報確保用の配列を準備
        Dim segments = New ArraySegment(Of Byte)(buffer)

        Send(segments)
    End Sub
    ''' <summary>
    ''' 送信メソッド
    ''' </summary>
    ''' <param name="segments"></param>
    Private Async Sub Send(ByVal segments As ArraySegment(Of Byte))

        'サーバーにリクエスト送るまで以降の処理は行わない
        Await ws.SendAsync(segments, WebSocketMessageType.Text, True, CancellationToken.None)

        Dim buffer(1024) As Byte

        '受信情報確保用の配列を準備
        Dim segment = New ArraySegment(Of Byte)(buffer)

        'サーバからのレスポンス情報を取得するまで待機状態
        Dim result = Await ws.ReceiveAsync(segment, CancellationToken.None)

        'エンドポイントCloseの場合、処理を中断

        'メッセージの最後まで取得
        Dim count As Int16 = result.Count

        '
        While (result.EndOfMessage = False)

            If (count >= buffer.Length) Then

                Await ws.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "That's too long", CancellationToken.None)

            End If

            segment = New ArraySegment(Of Byte)(buffer, count, buffer.Length - count)
            result = Await ws.ReceiveAsync(segment, CancellationToken.None)

            count += result.Count

        End While

        'メッセージを取得
        Dim message = Encoding.UTF8.GetString(buffer, 0, count)
        Debug.WriteLine(message)
        Label1.Text = message
    End Sub
    ''' <summary>
    ''' 受信メソッド
    ''' </summary>
    Private Async Sub Receive()

        While (True)


            Dim buffer(1024) As Byte

            '受信情報確保用の配列を準備
            Dim segment = New ArraySegment(Of Byte)(buffer)

            'サーバからのレスポンス情報を取得
            Dim result = Await ws.ReceiveAsync(segment, CancellationToken.None)

            'エンドポイントCloseの場合、処理を中断

            'メッセージの最後まで取得
            Dim count As Int16 = result.Count

            '↓これは何のために必要なのかいまいちわからない、メッセージを最後までの"最後"とは何を示すのか？
            '→チャンク送信だとデータをぶつ切れで送っているので、そういう場合を想定して必要
            '☆★☆チャンク送信の場合を考えている、実際にサーバー側でチャンク送信して、中の処理のテスト確認が必要☆★☆　　　
            While (result.EndOfMessage = False)

                If (count >= buffer.Length) Then

                    Await ws.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "That's too long", CancellationToken.None)

                End If

                segment = New ArraySegment(Of Byte)(buffer, count, buffer.Length - count)
                result = Await ws.ReceiveAsync(segment, CancellationToken.None)

                count += result.Count

            End While

            'メッセージを取得
            Dim message = Encoding.UTF8.GetString(buffer, 0, count)
            Debug.WriteLine(message)
            Label1.Text = message
        End While
    End Sub
End Class
