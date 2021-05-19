namespace TMG.TestTasks.Task1.Interfaces
{
    interface IRichTextBoxDriver
    {
        string GetText();
        void AppendText(string text, Color textColor);
        void ClearText();
    }
}
