# UI-O-Matic #

UI-O-Matic allows you to auto generate an integrated crud UI in Umbraco for a db table based on a [petapoco](http://www.toptensoftware.com/petapoco/) poco.

## How can I install it? ##
UI-O-Matic can be installed from the [Nuget package repository](https://www.nuget.org/packages/Nibble.Umbraco.UIOMatic/), or build manually from the [source-code](https://github.com/TimGeyssens/UIOMatic)

## Getting Started ##
Of course make sure that UI-O-Matic is installed and then create your poco

### Example  ###
If you have the following db table

    CREATE TABLE [People] (
      [Id] int IDENTITY (1,1) NOT NULL
    , [FirstName] nvarchar(255) NOT NULL
    , [LastName] nvarchar(255) NOT NULL
    , [Picture] nvarchar(255) NOT NULL
    );

And the following petapoco poco

    [TableName("People")]
    public class Person
    {
        public Person() { }

        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Picture { get; set; }

    }

The next additions to the class (attributes and interface)

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
    }

will generate the following crud UI

![](img/gettingstartedexample.png)

## Video demonstration ##

Watch UI-O-Matic on [uHangout EP72](https://www.youtube.com/watch?v=MUlAO85oQ4s)


