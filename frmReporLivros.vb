Imports System.Data.SqlClient
Public Class frmReporLivros
    Dim con As SqlConnection = Biblioteca.Conexao()

    Dim dtb As New DataTable
    Dim dtbLocal As New DataTable

    Public Sub AtualizarGrid()

        con.Open()
        Dim adp As New SqlDataAdapter("SELECT li.codigo AS [codigo], e.codigo AS [Codigo Emprestismo], li.Nome AS [Nome do Livro], e.dataDevolucao AS [Prazo da Devolução] FROM Emprestimo e INNER JOIN Livro li ON  li.codigo = e.codLivro INNER JOIN Leitor le ON le.codigo = e.codLeitor WHERE e.entregue = 'true' AND e.reposto = 'false'", con)
        dtb.Clear()
        adp.Fill(dtb)
        con.Close()

    End Sub

    Public Sub AtualizarLocal()
        Dim adp As New SqlDataAdapter("select l.codigo as [codigo] , p.capacidade AS [Capacidade Prateleira],  l.codEstante , p.posicao AS [Posição da Prateleira] , l.localização AS [Localização do Exemplar], l.codPrateleira FROM localizacao l INNER JOIN Prateleira p ON l.codPrateleira = p.codigo WHERE codLivro = @codLivro", con)
        adp.SelectCommand.Parameters.Add("@codLivro", SqlDbType.Int).Value = dtb.Rows(dtgRepor.CurrentRow.Index).Item("codigo")
        dtbLocal.Clear()
        adp.Fill(dtbLocal)
        con.Close()
    End Sub

    Private Sub btnVoltar_Click(sender As Object, e As EventArgs) Handles btnVoltar.Click
        Me.Close()
        frmMenu.Show()
    End Sub

    Private Sub fmrReporLivros_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        dtgRepor.DataSource = dtb
        dtgLocal.DataSource = dtbLocal

        AtualizarGrid()
        AtualizarLocal()

        dtgLocal.Columns("codigo").Visible = False
        dtgLocal.Columns("codEstante").Visible = False
        dtgLocal.Columns("codPrateleira").Visible = False
        dtgLocal.Columns("Capacidade Prateleira").Visible = False

        dtgRepor.Columns("codigo").Visible = False
        dtgRepor.Columns("Codigo Emprestismo").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        dtgRepor.Columns("Nome do Livro").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        dtgRepor.Columns("Prazo da Devolução").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill

        dtgLocal.Columns("Localização do Exemplar").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        dtgLocal.Columns("Posição da Prateleira").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
    End Sub

    Private Sub btnRepor_Click(sender As Object, e As EventArgs) Handles btnRepor.Click

        ''frmr confirma aqui 
        frmConfirma.Size = New Size(710, 300)
        frmConfirma.btnSIM.Location = New Point(145, 200)
        frmConfirma.btnNAO.Location = New Point(495, 200)
        frmConfirma.lblTexto.Text = "Tem certeza de que o livro esta mesmo no local indicado " + vbCrLf + "pelo sistema? Lembre-se de que o posicionamento exato do livro " + vbCrLf + "influencia no funcionamento do sistema como " + vbCrLf + "um todo e que é demasiado importante a sua cooperação " + vbCrLf + "para com a organização geral da biblioteca"
        frmConfirma.ShowDialog()

        If frmConfirma.DialogResult = Windows.Forms.DialogResult.Yes Then
            con.Open()
            Dim reposto As New SqlCommand("UPDATE emprestimo SET reposto = 'true' WHERE codigo = @codigo", con)
            reposto.Parameters.Add("@codigo", SqlDbType.Int).Value = CInt(dtb.Rows(dtgRepor.CurrentRow.Index).Item("Codigo Emprestismo"))
            reposto.ExecuteNonQuery()
            con.Close()
            ''MsgBox("reposto com sucesso")
        End If
        AtualizarGrid()
        AtualizarLocal()
    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click
        Me.Close()
        frmMenu.Show()
    End Sub

    Private Sub dtgRepor_Click(sender As Object, e As EventArgs) Handles dtgRepor.Click
        AtualizarLocal()
    End Sub
End Class