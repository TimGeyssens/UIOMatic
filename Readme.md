# UI-O-Matic #

[![Build status](https://ci.appveyor.com/api/projects/status/94932v6vx6mp2g57?svg=true)](https://ci.appveyor.com/project/TimGeyssens/uiomatic)
[![Documentation Status](https://readthedocs.org/projects/uiomatic/badge/?version=latest)](http://uiomatic.readthedocs.org/en/latest/)
[![NuGet release](https://img.shields.io/nuget/v/Nibble.Umbraco.UIOMatic.svg)](https://www.nuget.org/packages/Nibble.Umbraco.UIOMatic)
[![Our Umbraco project page](https://img.shields.io/badge/our-umbraco-orange.svg)](https://our.umbraco.org/projects/developer-tools/ui-o-matic/)
[![Chat on Gitter](https://img.shields.io/badge/gitter-join_chat-green.svg)](https://gitter.im/TimGeyssens/UIOMatic)

**Auto generate an integrated crud UI in Umbraco for a db table based on a [petapoco ](http://www.toptensoftware.com/petapoco/)poco**

![](logo.png)

Implement an interface and decorate your class and properties with some additional attributes.

## Example ##
If you have the following db table

    CREATE TABLE [People] (
      [Id] int IDENTITY (1,1) NOT NULL
    , [FirstName] nvarchar(255) NOT NULL
    , [LastName] nvarchar(255) NOT NULL
    , [Picture] nvarchar(255) NOT NULL
    );

This class

    [UIOMaticAttribute("People","icon-users","icon-user")]
    [TableName("People")]
    public class Person: IUIOMaticModel
    {
        public Person() { }

        [UIOMaticIgnoreField]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [UIOMaticField("First name","Enter the persons first name")]
        public string FirstName { get; set; }

        [UIOMaticField("Last name", "Enter the persons last name")]
        public string LastName { get; set; }

        [UIOMaticField("Picture", "Select a picture", View = "file")]
        public string Picture { get; set; }

        public override string ToString()
        {
            return FirstName + " " + LastName;
        }

        public IEnumerable<Exception> Validate()
        {
            var exs = new List<Exception>();

            if(string.IsNullOrEmpty(FirstName))
                exs.Add(new Exception("Please provide a value for first name"));

            if (string.IsNullOrEmpty(LastName))
                exs.Add(new Exception("Please provide a value for last name"));


            return exs;
        }
    }`

Will generate the following UI

![](example.png)

## Documentation ##

For docs please go to[ http://uiomatic.readthedocs.org/](http://uiomatic.readthedocs.org/)

### Test site ###
Backoffice credentials: 
- tim@nibble.be / password


