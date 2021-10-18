# <img src="docs/img/uiomatic.png?raw=true" width="300"> #

[![Build status](https://ci.appveyor.com/api/projects/status/48f75yfhb4htigw7?svg=true)](https://ci.appveyor.com/project/TimGeyssens/uiomatic-dotnetcore)
[![NuGet release](https://img.shields.io/nuget/v/Nibble.Umbraco.UIOMatic.svg)](https://www.nuget.org/packages/Nibble.Umbraco.UIOMatic)

Please be aware of the [License Model](https://github.com/TimGeyssens/UIOMatic/blob/v3/LICENSE.md) before using this package since additional charges might apply if you are an Umbraco Gold Partner. And it is prohibited to use the Software on the Cloud offering of Umbraco.

**Auto generate an integrated crud UI in Umbraco for a db table based on a [npoco ](<https://github.com/schotime/NPoco>)poco**

Simply decorate your class and properties with some additional attributes.

## Example ##
If you have the following db table

    CREATE TABLE [People] (
      [Id] int IDENTITY (1,1) NOT NULL
    , [FirstName] nvarchar(255) NOT NULL
    , [LastName] nvarchar(255) NOT NULL
    , [Picture] nvarchar(255) NOT NULL
    );

This class

    [UIOMatic("people","People","Person", FolderIcon = "icon-users", ItemIcon = "icon-user")]
    [TableName("People")]
    public class Person
    {
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }
    
    	[Required]
        [UIOMaticField(Name = "First name", Description = "Enter the persons first name")]
        public string FirstName { get; set; }
    
    	[Required]	
        [UIOMaticField(Name = "Last name",Description = "Enter the persons last name")]
        public string LastName { get; set; }
    
        [UIOMaticField(Name = "Picture",Description = "Select a picture", View = UIOMatic.Constants.FieldEditors.File)]
        public string Picture { get; set; }
    
        public override string ToString()
        {
            return FirstName + " " + LastName;
        }
    
    }

Will generate the following UI

![](docs/img/examplev8.png?raw=true)

## License ##
Important before using please make sure you read the license: since charges might apply [AholeCloud](https://github.com/TimGeyssens/UIOMatic/blob/v3/LICENSE.md)

## Documentation ##
For the full documentation please go to [https://timgeyssens.gitbook.io/ui-o-matic/](https://timgeyssens.gitbook.io/ui-o-matic//)
## The Team ##

* [Tim Geyssens](https://github.com/TimGeyssens)
* [Other Contributors](https://github.com/TimGeyssens/UIOMatic/graphs/contributors)
