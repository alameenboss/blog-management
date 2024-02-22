# To Run API Locally 

Please check for the connection string. appsetting.developement.json has both connection string for 

    1. (localdb)\mssqllocaldb
    2. .\sqlserver

Once the connection string is properly configured then Api will automatically apply mogration for Identity server DB and BlogDb.

- Used Seperate DBContext for Identity Server and BlogDb but same Database

- For the purpose of sample the source is not split into multiple porjects, but can be easily split

### Features
- Used Inmemory cache
- Mailkit to send email
- User and Role creation
- Register user, Login , forgot password, email confirmation flow
- EFcore
- Serilog for logging
- Identity Server
- JWT Token for authorization
- Swagger documentation
- Auto mapper

# Migration of UserDBContext

For Adding new migration
 `Add-Migration -Name "Initial" -OutputDir "IdentityServer\Migrations" -Project "Applogiq" -Context "UserDbContext"`
 
To update the Database
 `Update-Database -Project "Applogiq" -Context "UserDbContext"`

# Migration of BlogDbContext

For Adding new migration
 `Add-Migration -Name "Initial" -OutputDir "BlogModule\Migrations" -Project "Applogiq" -Context "BlogDbContext"`

To update the Database
`Update-Database -Project "Applogiq" -Context "BlogDbContext"`


# There are two roles

1. Super Admin with following credentials

```
email : superadmin@Applogiq
password : Applogiq@software0211
```

Token Request
``` json
{
  "email": "superadmin@Applogiq",
  "password": "Applogiq@software0211"
}
```

2. Member with following credentials
```
email : member@Applogiq
password : Applogiq@software0211
```

Token Request
``` json
{
  "email": "member@Applogiq",
  "password": "Applogiq@software0211"
}
```

# Blog

- GetblogById and FilterBlog doesnot have any authorization
- Filter blog has pagegination with optional filter by author or category
- To create or update a blog user should have a valid jwt token

### Create Blog

Sample Create Blog Request
``` json
{
  "title": "Introduction to CSharp",
  "content": "CSharp is a Programing Language from Microsoft",
  "category": "CSharp",
  "publishDate": "2024-02-21T17:58:34.214Z",
  "author": "alameen"
}
```
``` json
{
  "title": "Hello World in CSharp",
  "content": "consolo.writeline('Hello World');",
  "category": "CSharp",
  "publishDate": "2024-02-21T17:58:34.214Z",
  "author": "alameen"
}
```

### Update Blog

Sample Update Blog Request
``` json
{
"id": 2,
  "title": "Hello Earth in CSharp",
  "content": "consolo.writeline('Hello Earth');",
  "category": "CSharp",
  "publishDate": "2024-02-21T17:58:34.214Z",
  "author": "kumar"
}
```

### To Delete a blog 

 - the user should be a super admin
 - To Delete simply pass the id of the blog



# Comment

- GetCommnetByBlogId will give all the comments of a particular blog
- Create comment for a particular blog by passing blog id

### Create Comment

Sample Create Comment Request
``` json
{
  "content": "First Comment",
  "author": "alameen",
  "blogId": 1
}
```

### To Delete a Comment 

 - the user should be a super admin
 - To Delete simply pass the id of the comment

