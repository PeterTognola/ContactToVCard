# ContactToVCard

## What is ContactToVCard

A small, completely offline, multi-platform app to convert Microsoft Contact (`.CONTACT`) files to VCard files (`.VCF`).

## Quick Start

To get started all you need to do is download the executable program files from [here](https://github.com/PeterTognola/ContactToVCard/releases).

## Building

Built using C# .NET 10, all you need is the sdk installed (can be checked via `dotnet --version`) or downloaded from [here](https://dotnet.microsoft.com/en-us/download).

Once installed, simply clone the repo then build and run inside the root folder via `dotnet build && dotnet run`.

## What's Supported

This project is in active development, so not everything in .CONTACT files are supported. Below is a list of what is and isn't supported

| Contact Data | VCard Support  | CSV Support |
| ---- | ----------------- | ----- | --- |
| First Name | :white_check_mark: | :x: |
| Last Name | :white_check_mark: | :x: |
| Other Names | :x: | :x: |
| Email Address | :white_check_mark: | :x: |
| Addresses | :white_check_mark: | :x: |
| Date of Birth | :x: | :x: |
| Anniversary | :x: | :x: |
| Phone Numbers | :white_check_mark: | :x: |

> If otherwise stated, elements like "Other Address" that are their own entity will be merged with the corresponding VCF element via a type.

## Rationale

The reason for this project is I came across some old .CONTACT files while going through some old harddrives. I wanted to move them to my phone but unfortunately this file type is not really supported by modern phones (Android or iPhone).

The only solutions online I could find either were via a website (which I wouldn't trust, due to the sensitivity of the data), using Outlook, or via Powershell scripts/Excel.

So I thought I'd put together a simple to use app to do this (and hadn't had the opportunity to work with XAML/MVVM for a few years).

## Roadmap

| Feature | Status | Estimate |
| ------- | ------ | -------- |
| todo    | todo   | todo     |

## Contributing

todo.
