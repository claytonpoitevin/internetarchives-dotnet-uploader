# internetarchives-dotnet-uploader

This project was made for me to upload my CD / DVD collection backups to archive.org using the command-line tool on Linux.
May work on Windows too using WSL.

Usage:
dotnet run /path/to/files

It will scan for the file '_metadata.json_' in every folder on the parameter path. If a folder contains the _metadata.json_ file, a new **archive.org** item will be created with the metadata on the json file. The example metadata file is attached on this repository.

It is necessary that you have already configured **ia** (ia configure) for it to work.
I recommend uploading to the collection 'test_collection' first to see how this works and then upload to your prefered collection.

##TODOs

* Better 'subject' metadata implementation
* Implement more metadata
* Port to different platforms?

Sources:
[Internet Archive CLI](https://archive.org/services/docs/api/internetarchive/cli.html)