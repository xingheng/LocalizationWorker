LocalizationWorker
==================

[Mono][C#] A command line tool which is used for searching and/or replacing all the Chinese unicode strings in specified directory for project which is written by Objective-C language.

###Environment

OSX and [mono](http://www.mono-project.com/). Currently I just validated it in Mavericks.

###How to use it?

mono [-d|--dir] <your_project_directory_full_path> [-a|--action search|replace] [-o|--output <search_result_output_file>] [-f|--format kv|kkv] [-v|--verbose]
