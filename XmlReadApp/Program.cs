using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XmlReadApp
{
    class Program
    {

        static MyContacts myContacts;
        static Phone myPhone;
        static List<MyContacts> myContactList;
        static List<Phone> listPhone; 
        
        static void Main(string[] args)
        {
            String path = @"..\..\contacts.xml";
            ParseXML(path);
            var connectionString = "mongodb://localhost";
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            var database = server.GetDatabase("test");
            var col = database.GetCollection<MyContacts>("contacts");

            Console.WriteLine("Kayit Basladi");
                foreach (MyContacts n in myContactList)
                {

                    var query = Query.And(
                                Query.EQ("name", n.name),
                                Query.EQ("lastname", n.lastname)
                    );

                    var cursor = col.FindOne(query);
                    if (cursor != null)
                    {
                       
                        col.Save(n);
                    }
                    else
                    {
                    
                        foreach (Phone telefon in listPhone)
                        {
                            if (telefon.id == n.id)
                            {
                                if (telefon.phone != n.phones)
                                {
                                    n.phones +=","+telefon.phone;
                                }
                            }

                        }
                        col.Insert(n);
                        Console.WriteLine(n.id);
                    }

                }

                Console.WriteLine("basari ile kaydedildi");
   
            //yaz();
            Console.ReadLine();

       }


        static void yaz() {

            foreach (MyContacts mynesne in myContactList)
            {
                Console.WriteLine("id : " + mynesne.id);
                Console.WriteLine("Name :" + mynesne.name);
                Console.WriteLine("Last Name :" + mynesne.lastname);
                Console.WriteLine("Telefon :");
                foreach(Phone telefon in listPhone) {
                    if (telefon.id == mynesne.id) {
                        Console.Write(telefon.phone + ";");
                    }
                   
                }
                Console.WriteLine();
            }
        
        }

        static void ParseXML(String filename) {

            XmlDocument xml = new XmlDocument();
            myContacts = new MyContacts();
            myPhone = new Phone();
            List<String> listContacts = new List<String>();
            listPhone = new List<Phone>();
            myContactList = new List<MyContacts>();
            StringBuilder sb = new StringBuilder();
            string keyvalue;

            try
            {
                xml.Load(filename);

                XmlNode allContacts = xml.SelectSingleNode("contacts");
              
                XmlNodeList contactList = allContacts.SelectNodes("contact");

                int i = 0;
                int idd = 0;
                foreach (XmlNode contact in contactList)
                {
                  
                    string name = contact.SelectSingleNode("name").InnerXml;
                    string lastname = contact.SelectSingleNode("lastName").InnerXml;
                    string phone = contact.SelectSingleNode("phone").InnerXml;

                    keyvalue = name+"-"+lastname;
              
                    if (listContacts.Contains(keyvalue))
                    {
                      if(name == myContacts.name && lastname==myContacts.lastname){
                       idd = myContacts.id;
                      }
                      myPhone = new Phone(idd,phone);
                      allContacts.RemoveChild(contact);
                    }
                    else
                    {
                        myContacts = new MyContacts();
                        myContacts.id = i;
                        myContacts.name = name;
                        myContacts.lastname = lastname;
                        myPhone = new Phone(i,phone);
                        myContacts.phones = myPhone.phone;

                        listContacts.Add(keyvalue);
                        i++;
                    }
                   
                    myContactList.Add(myContacts);
                    listPhone.Add(myPhone);
                    
        
                }

         
            }catch(Exception ex){
                throw ex;
            }
            
        }


        class  MyContacts {

        public MyContacts() { }

        public int id { get; set; }
        public string name { get; set; }
        public string lastname { get; set; }
        public string phones {get; set;}

        }

        class Phone {

            public int id {get; set;}
            public string phone {get; set;}
        
            public Phone(){}

            public Phone(int id, string phone){
                this.id = id;
                this.phone = phone;
            }

        }

    

    }
  
}
