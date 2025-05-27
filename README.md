# ChatGPT Extractor

A command-line tool to extract and convert ChatGPT conversations from the official JSON export format into individual markdown files.

## Description

ChatGPT Extractor is a .NET-based utility that processes the `conversations.json` file exported from ChatGPT and converts each conversation into a separate markdown file. The tool preserves the conversation structure, formatting, and timestamps while creating easily readable markdown files.

## Features

- Extracts individual conversations from ChatGPT's JSON export
- Converts conversations to markdown format
- Preserves conversation structure and formatting
- Maintains chronological order with file timestamps
- Handles special characters and invalid filename characters
- Supports verbose logging for detailed operation information
- Optional index prefixing for file names

## Installation

1. Ensure you have .NET 9.0 SDK installed
2. Clone this repository
3. Install the tool:
```bash
./install.ps1 # Windows
./install.sh # Linux/OSX
```

## Usage

```bash
chat-gpt extract -p <path-to-conversations.json> -o <output-directory> [options]
```

### Parameters

- `-p, --path`: Path to the conversations.json file (required)
- `-o, --output`: Destination folder for extracted conversations (required)
- `-u, --user`: Your user name (optional, default User)
- `-i, --include-index`: Include index in file names (optional, default: false)
- `-v, --verbose`: Enable verbose mode for detailed logging (optional, default: false)

### Example

```bash
chat-gpt extract -p "./conversations.json" -o "./Exports" -v
```

## Output Format

Each conversation is saved as a separate markdown file with the following format:

```markdown
## User
[User's message]

## ChatGPT
[ChatGPT's response]
```

## Error Handling

The tool handles various error conditions:
- Missing input file
- Invalid JSON format
- File permission issues
- Invalid file paths

## Contributing  

Contributions are welcome! Please follow these steps:  

1. Fork the repository.  
2. Create a new branch for your feature or bug fix.  
3. Commit your changes and push them to your fork.  
4. Submit a pull request.  

## License  

This project is licensed under the GPL-3.0 License. See the `LICENSE` file for details.  
