namespace ClipboardManager.Models;


public class ClipboardEntry
{
    public int Id { get; set; }
    public DateTime Time { get; set; }
    public string Text { get; set; }
    public DateTime CopiedAt { get; set; } = DateTime.Now;

    public ClipboardEntry(string text)
    {
        Text = text;
        Time = DateTime.Now;
    }

    // public bool Equals(ClipboardEntry? other)
    // {
    //     if (other is null) return false;
    //     return Text == other.Text;
    //     
    // }

    public override bool Equals(object obj)
    {
        if (obj is ClipboardEntry other)
        {
            return Text == other.Text;
        }
        return false;
    }
    
}