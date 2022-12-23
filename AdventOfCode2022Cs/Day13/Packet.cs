using System.Text;

namespace AdventOfCode2022Cs.Day13;

public abstract class DividerPacket : Packet
{
    public DividerPacket(string data) : base(data)
    {
    }
}

public class DividerPacketA : DividerPacket
{
    public DividerPacketA() : base("[[2]]")
    {
    }
}

public class DividerPacketB : DividerPacket
{
    public DividerPacketB() : base("[[6]]")
    {
    }
}

public class Packet
{
    public List<object> Data { get; }

    public Packet(string data)
    {
        data = data.Replace("[", " [ ").Replace("]", " ] ");

        var split = data.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
        Data = ParseData(split);
    }

    private List<object> ParseData(string[] parts)
    {
        var stack = new Stack<List<object>>();

        List<object> current = new List<object>();
        stack.Push(current);

        foreach (var part in  parts      )
        {
            if (part == "[")
            {
                current = new List<object>();
                stack.Push(current);
            }else if (part == "]")
            {
                var done = stack.Pop();
                stack.Peek().Add(done);
            }
            else
            {
                var val = Convert.ToInt32(part);
                stack.Peek().Add(val);
            }
        }
            
        return stack.Pop().First() as List<object> ?? throw new InvalidOperationException();
    }

    public override string ToString() => DataToString(Data);

    private string DataToString(List<object> data)
    {
        var sb = new StringBuilder();

        sb.Append("[");

        foreach (object o in data)
        {
            if (o is int i)
            {
                sb.Append(i.ToString());
            }
            else if (o is List<object> d)
            {
                sb.Append(DataToString(d));
            }
            else
            {
                throw new InvalidOperationException($"Unexpected value in data: {o}");
            }

            if (data.Last() != o)
            {
                sb.Append(",");
            }
        }

        sb.Append("]");

        return sb.ToString();
    }
}