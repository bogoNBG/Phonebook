using System;
using System.Collections.Generic;
using System.Data.SQLite;
using WpfApp4.Model;

namespace WpfApp4.Repository
{
    class MainRepository
    {
        public const string dbfile = "URI=file:SQLiteDB.db";
        public string commandLine;

        void ConnectToTable(string commandLine)
        {
            using SQLiteConnection connection = new SQLiteConnection(dbfile);
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(commandLine, connection)) 
                { 
                    command.ExecuteNonQuery(); 
                }
            }
        }

        bool Check(string tableName)
        {
            using (SQLiteConnection connection = new SQLiteConnection(dbfile))
            {
                connection.Open();

                string checkTableQuery = "SELECT name FROM sqlite_master WHERE type='table' AND name=@TableName;";
                using (SQLiteCommand command = new SQLiteCommand(checkTableQuery, connection))
                {
                    command.Parameters.AddWithValue("@TableName", tableName);

                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public void CreateTables()
        {
            if ( !(this.Check("Contacts") && this.Check("Options") && this.Check("Links")) )
            {
                this.commandLine = "create table Contacts (ID integer primary key autoincrement not null unique, NAME text, NUMBER text, EMAIL text);";
                this.ConnectToTable(this.commandLine);
                this.commandLine = "create table Options (ID integer primary key autoincrement not null unique, NAME text);";
                this.ConnectToTable(this.commandLine);
                this.commandLine = "create table Links (ID integer primary key autoincrement not null unique, [CONTACT ID] integer, [OPTION ID] integer, NAME text);";
                this.ConnectToTable(this.commandLine);
            }
        }

        public void AddRow(Contact contact)
        {
            this.commandLine = $"insert into Contacts (NAME,NUMBER,EMAIL) values ('{contact.Name}','{contact.Number}','{contact.Email}');";
            this.ConnectToTable(this.commandLine);
        }
        public void DeleteRow(int contactId)
        {
            this.commandLine = $"delete from Contacts where ID={contactId};";
            this.ConnectToTable(this.commandLine);
        }
        public void UpdateRow(string name, string number, string email, int contactId)
        {
            this.commandLine = $"update Contacts set NAME='{name}', NUMBER='{number}', EMAIL='{email}' where ID={contactId}";
            this.ConnectToTable(this.commandLine);
        }

        public void AddOption(Option option)
        {
            this.commandLine = $"insert into Options (NAME) values ('{option.Name}');";
            this.ConnectToTable(this.commandLine);
        }

        public void RemoveOption(int optionId)
        {
            this.commandLine = $"delete from Options where ID='{optionId}'";
            this.ConnectToTable(this.commandLine);
        }

        public void AddLink(int contactId, int optionId, string linkValue)
        {
            this.commandLine = $"insert into Links ([CONTACT ID], [OPTION ID], NAME) VALUES ({contactId},{optionId},'{linkValue}')";
            this.ConnectToTable(this.commandLine);
        }

        public void RemoveLink(int linkId)
        {
            this.commandLine = $"delete from Links where ID={linkId};";
            this.ConnectToTable(this.commandLine);
        }

        public void UpdateLink(string linkValue, int linkId)
        {
            this.commandLine = $"update Links set NAME='{linkValue}' where ID={linkId}";
            this.ConnectToTable(this.commandLine);
        }

        public void RemoveLinksFromContact(int contactId)
        {
            this.commandLine = $"delete from Links where [CONTACT ID]={contactId};";
            this.ConnectToTable(this.commandLine);
        }

        public void RemoveLinksFromOptions(int optionId)
        {
            this.commandLine = $"delete from Links where [OPTION ID]={optionId};";
            this.ConnectToTable(this.commandLine);
        }

        public List<Link> GetContactsLinksFromDB(Contact contact)
        {
            List<Link> links = new();
            using (SQLiteConnection connection = new(dbfile))
            {
                connection.Open();
                SQLiteCommand cmd = new SQLiteCommand($"select * from Links where [CONTACT ID] = {contact.Id}", connection);
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        int contactId = reader.GetInt32(1);
                        int optionId = reader.GetInt32(2);
                        string name = reader.GetString(3);

                        links.Add(new Link(id, contactId, optionId, name));
                    }
                }
            }

            return links;
        }

        public static List<Option> GetOptionsFromDB()
        {
            List<Option> options = new();
            using (SQLiteConnection connection = new(dbfile))
            {
                connection.Open();

                string queryOptions = "SELECT ID, NAME FROM Options;";
                SQLiteCommand commandOptions = new(queryOptions, connection);

                using (SQLiteDataReader readerOptions = commandOptions.ExecuteReader())
                {
                    while (readerOptions.Read())
                    {
                        int id = readerOptions.GetInt32(0);
                        string name = readerOptions.GetString(1);

                        options.Add(new Option(id, name));
                    }
                }
            }

            return options;
        }

        public static Option GetOptionByIdFromDB(int optionId)
        {
            using (SQLiteConnection connection = new(dbfile))
            {
                connection.Open();

                string queryOptions = $"SELECT ID, NAME FROM Options where id = {optionId};";
                SQLiteCommand commandOptions = new(queryOptions, connection);

                using (SQLiteDataReader readerOptions = commandOptions.ExecuteReader())
                {
                    while (readerOptions.Read())
                    {
                        int id = readerOptions.GetInt32(0);
                        string name = readerOptions.GetString(1);

                        return new Option(id, name);
                    }
                }          
                
                return null;
            }
        }       

        public static List<Contact> GetContactsFromDB()
        {
            List<Contact> contacts = new();
            using (SQLiteConnection connection = new(dbfile))
            {
                connection.Open();

                string queryContacts = "SELECT ID, NAME, NUMBER, EMAIL FROM Contacts;";

                SQLiteCommand commandContacts = new(queryContacts, connection);

                using (SQLiteDataReader readerContacts = commandContacts.ExecuteReader())
                {
                    while (readerContacts.Read())
                    {

                        int id = readerContacts.GetInt32(0);
                        string name = readerContacts.GetString(1);
                        string number = readerContacts.GetString(2);
                        string email = readerContacts.GetString(3);

                        Contact contact = new(id, name, number, email);

                        string queryLinks = $"SELECT ID, [CONTACT ID], [OPTION ID], NAME FROM Links WHERE [CONTACT ID] = {contact.Id};";
                        SQLiteCommand commandLinks = new(queryLinks, connection);
                        using (SQLiteDataReader readerLinks = commandLinks.ExecuteReader())
                        {
                            while (readerLinks.Read())
                            {
                                int linkId = readerLinks.GetInt32(0);
                                int contactId = readerLinks.GetInt32(1);
                                int optionId = readerLinks.GetInt32(2);
                                string linkName = readerLinks.GetString(3);

                                contact.Links.Add(new Link(linkId, contactId, optionId, linkName));
                            }
                        }

                        contacts.Add(contact);
                    }
                }

                return contacts;
            }
        }
    }
}