using Contracts.Messages;
namespace Test;

public static class Test {

    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    private static readonly string[] _names = new string[]{
    "Liam", "Emma", "Noah", "Olivia", "William", "Ava", "James", "Isabella", "Oliver", "Sophia",
    "Benjamin", "Mia", "Elijah", "Charlotte", "Lucas", "Amelia", "Mason", "Harper", "Logan", "Evelyn",
    "Alexander", "Abigail", "Ethan", "Emily", "Jackson", "Elizabeth", "Aiden", "Mila", "Sebastian", "Ella",
    "Michael", "Avery", "Carter", "Sofia", "Jacob", "Camila", "Daniel", "Aria", "Matthew", "Scarlett",
    "Henry", "Victoria", "Joseph", "Madison", "Samuel", "Luna", "David", "Grace", "Owen", "Chloe"
};

    private static string GetRandomName() {
        var random = new Random();
        return _names[random.Next(_names.Length)];
    }
    private static string GetRandomString(int length) {
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public static MemberQrRead CreateMessageQrRead() {
        return new MemberQrRead {
            MemberId = GetRandomString(10),
            CenterId = GetRandomString(10),
            DateOfArrival = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
            EmailData = new EmailData {
                Members = new[] { GetRandomString(10), GetRandomString(10), GetRandomString(10) },
                Subject = GetRandomString(50),
            },
        };
    }

    public static UserCreated CreateUserCreated() {
        return new UserCreated {
            Email = String.Format("{0}@{1}.com", GetRandomName(), GetRandomString(5)),
            Password = GetRandomString(10),
            UserId = GetRandomString(10),
        };
    }
}