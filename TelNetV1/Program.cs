// See https://aka.ms/new-console-template for more information
//Programmed by Brenden Scarfone 
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using WindowsInput.Native;
using WindowsInput;
using System.IO;
using System.IO.Pipes;
//0x03, close

//init vars
var Location = ".";
string Route = Path.GetFullPath(Location);
var ItemDir = Path.Combine(Route, "cfg.ini");
bool IniExists = File.Exists(ItemDir);



Console.Title = "Telnet++";
Encoding ASEN = new ASCIIEncoding();
byte[] EnterKey = {0x0A};//, 0x06, 0x07, 0x07, 0x08,0x09, 0x0A, 0x0b, };
byte[] CarriageReturn = { 0x0D };//, 0x06, 0x07, 0x07, 0x08,0x09, 0x0A, 0x0b, };
byte[] buffer = new byte[65535];
string ret;
string input;
int state = 1;
Stream stm;
int shortwait = 300;
string DeviceIP = " ";
int Waittime = 1399;
bool Live = true;
InputSimulator sim = new InputSimulator();
string Username = " ";
string Password = " ";
string DefaultIP = " ";
//init vars




void CreateIni()
{
    Console.ForegroundColor = ConsoleColor.Blue;
    Console.WriteLine("No cfg.ini found, creating one now.");
    Console.WriteLine("Default IP");
    Console.ForegroundColor = ConsoleColor.Red;
    DefaultIP = Console.ReadLine();
    Console.ForegroundColor = ConsoleColor.Blue;
    Console.WriteLine("Default Username");
    Console.ForegroundColor = ConsoleColor.Red;
    Username = Console.ReadLine();
    Console.ForegroundColor = ConsoleColor.Blue;
    Console.WriteLine("Default Password");
    Console.ForegroundColor = ConsoleColor.Red;
    Password = Console.ReadLine();


    using (FileStream fs = File.Create("CFG.ini"))
    {
        // Add some text to file    
        Byte[] IP = new UTF8Encoding(true).GetBytes(DefaultIP + "\n");
        fs.Write(IP, 0, IP.Length);
        byte[] user = new UTF8Encoding(true).GetBytes(Username + "\n");
        fs.Write(user, 0, user.Length);
        byte[] pass = new UTF8Encoding(true).GetBytes(Password + "\n");
        fs.Write(pass, 0, pass.Length);
        fs.Close();
    }

}

void LoadIni()
{
    string[] lines = File.ReadAllLines(ItemDir);

    DeviceIP = lines[0];
    Username = lines[1];
    Password = lines[2];
    
   
}




void MainCode()
{

    

    Console.ForegroundColor = ConsoleColor.Blue;
    Console.Write("If all data is not captured during session ");


    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("PUSH ENTER");
    Console.ForegroundColor = ConsoleColor.Blue;
    Console.WriteLine("To Update Login, see CFG.ini");
    Console.Write("To Close Session, type");
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(" break");
    Console.ForegroundColor = ConsoleColor.Blue;
    Console.Write("To Clear the screen, type");
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(" clear");

    Console.ForegroundColor = ConsoleColor.Blue;
    Console.Write("If you get stuck at Logon Screen, type");
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(" return");

    Console.ForegroundColor = ConsoleColor.Blue;
    Console.WriteLine("Device Port: ");
    Console.ForegroundColor = ConsoleColor.Red;
    int DevicePort = Convert.ToInt16(Console.ReadLine());
    TcpClient client = new TcpClient(DeviceIP, DevicePort);
    stm = client.GetStream();
    stm.Write(EnterKey, 0, EnterKey.Length);

    AutoLogin();

   
    while (state == 1)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        RetreiveData();
        Console.Write(ret + " ");
        Console.ForegroundColor = ConsoleColor.Red;
        input = Console.ReadLine();
        if (input == "" || input == null) { input = " "; } //eeror handlng;
        if (input == "BREAK" || input == "break" || input == "Break") {Console.Clear(); client.Close(); return; }
        if (input == "CLEAR" || input == "clear" || input == "Clear") { Console.Clear(); input = " "; } //eeror handlng;
        if (input == "RETURN" || input == "return" || input == "Return") {stm.Write(CarriageReturn, 0 ,CarriageReturn.Length); }
        SendData();

    }



    void SendData()
    {
        buffer = new byte[65535]; //clear buffer
        stm = client.GetStream();
        buffer = ASEN.GetBytes(input);
        stm.Write(buffer, 0, buffer.Length);
        stm.Write(EnterKey, 0, EnterKey.Length);


    }

    void RetreiveData()
    {

        buffer = new byte[65535]; //clear buffer
        Thread.Sleep(Waittime);
        Thread.Sleep(shortwait);
        stm = client.GetStream();
        int data = stm.Read(buffer, 0, buffer.Length);
        ret = ASEN.GetString(buffer);

        ret = ret.TrimEnd(Convert.ToChar(0x0D));
    }

    void AutoLogin()
    {
        stm.Write(EnterKey, 0, EnterKey.Length);
        //stm.Write(CarriageReturn, 0, CarriageReturn.Length);
        Console.Write(".");

        buffer = ASEN.GetBytes(Username);
        stm.Write(buffer, 0, buffer.Length);
        stm.Write(EnterKey, 0, EnterKey.Length);
        //stm.Write(CarriageReturn, 0, CarriageReturn.Length);
        Thread.Sleep(shortwait);
        Console.Write(".");

        buffer = ASEN.GetBytes(Password);
        stm.Write(buffer, 0, buffer.Length);
        stm.Write(EnterKey, 0, EnterKey.Length);
        Thread.Sleep(Waittime);
        Console.Write(".");

        Thread.Sleep(Waittime); 
        stm.Write(CarriageReturn, 0, CarriageReturn.Length);
        Console.Write(".");






        Console.Clear();

    }
}


void runtime()
{

    if (IniExists == false)
    {
        CreateIni();
    }

    LoadIni();

    while (Live == true)
    {
        MainCode();

    }


}

runtime();