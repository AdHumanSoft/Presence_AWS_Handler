namespace Consumer.Firebase.Schema;

using Google.Cloud.Firestore;
using System.Collections.Generic;

[FirestoreData]
public class Admins 
{
    [FirestoreProperty("name")]
    public string Name { get; set; } = "";

    [FirestoreProperty("phone")]
    public string Phone { get; set; } = "";

    [FirestoreProperty("email")]
    public string Email { get; set; } = "";
}

[FirestoreData]
public class Contacts 
{
    [FirestoreProperty("genere")]
    public int Genere { get; set; } = 0;

    [FirestoreProperty("name")]
    public string Name { get; set; } = "";

    [FirestoreProperty("email")]
    public string Email { get; set; } = "";

    [FirestoreProperty("phone")]
    public string Phone { get; set; } = "";
}

[FirestoreData]
public class Members 
{
    [FirestoreProperty("email")]
    public string Email { get; set; } = "";

    [FirestoreProperty("finger_prints")]
    public List<string> FingerPrints { get; set; } = new List<string>();

    [FirestoreProperty("phone")]
    public string Phone { get; set; } = "";

    [FirestoreProperty("name")]
    public string Name { get; set; } = "";

    [FirestoreProperty("contact_rules")]
    public ContactRules ContactRules { get; set; } = new ContactRules();
}

[FirestoreData]
public class ContactRules 
{
    [FirestoreProperty("all")]
    public List<string> All { get; set; } = new List<string>();

    [FirestoreProperty("on_qr_read")]
    public List<string> OnQrRead { get; set; } = new List<string>();

    [FirestoreProperty("on_finger_match")]
    public List<string> OnFingerMatch { get; set; } = new List<string>();
}

[FirestoreData]
public class Centers 
{
    [FirestoreProperty("admins")]
    public List<string> Admins { get; set; } = new List<string>();

    [FirestoreProperty("name")]
    public string Name { get; set; } = "";
}

[FirestoreData]
public class Entities 
{
    [FirestoreProperty("admins")]
    public List<string> Admins { get; set; } = new List<string>();

    [FirestoreProperty("name")]
    public string Name { get; set; } = "";
}
