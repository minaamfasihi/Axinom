# Axinom
Upload a zip file, and get the list of all the items in json

This has 2 projects.
`Axinom.ControlPanel` and `Axinom.DataManagement`.

1) Clone this repo.
2) Open a terminal window and go inside `Axinom.ControlPanel`. You might have to run `dotnet restore`. After that, run `dotnet run`.
3) Open another terminal window and go inside `Axinom.DataManagement`. Remaining steps are same as above.
4) Now you have two apps running. `Axinom.ControlPanel` on `https://localhost:5001/` and `Axinom.DataManagement` on `https://localhost:5200/`
5) Go to `https://localhost:5001/` and enter `fasihi` for username and `Axinom@123` for password. And then click on upload file to upload any zip file. After that, click on submit. This will make a JSON of all the filenames (tree hierarchy) and then it will encrypt the entire JSON using AESEncryption `https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.aes?view=netcore-2.1`. Another option was to separately encrypt the filenames of each file and retain the JSON structure. On the `Axinom.DataManagement` side, it will have to iterate over all the names and nested files and decrypt all of them. This was time-consuming but probably this is how it's done usually.
6) The request comes to `Axinom.DataManagement`. It's an asp.net core web api project. Inside `Axinom.DataManagement`, we match the username and password from basic auth with the username and password present in `appsettings.json`. If they match, we proceed with decrypting the encrypted string and then deserialize it into a JSON object and then writing it to a file in the current directory.
7) I have created a Dockerfile and .dockerignore files but couldn't get it to run properly.

# Notes
- For creating the tree structure of filenames in JSON, I followed this: `https://www.youtube.com/watch?v=B8KUknLT6qU`
