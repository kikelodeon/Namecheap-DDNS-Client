# Namecheap DDNSUpdater

Dynamic DNS Updater for Namecheap domains.

## Description

This application is designed to update the dynamic IP address associated with your Namecheap domain. It ensures that your domain always points to the correct IP address, even if your internet connection's IP address changes.

## Features

## Features

- **Multiple IP Detection Providers:** 🔄 The application supports various IP detection providers, ensuring robustness in case one is temporarily unavailable. It automatically switches to an alternative provider among the options (you can easily add more as needed). This feature enhances reliability and flexibility in obtaining accurate IP address information.

- **Automated Dynamic IP Address Update:** 🚀 Automatically updates the dynamic IP address associated with your Namecheap domains, ensuring your domains always point to the correct IP address.

- **Support for Multiple Hosts and Domains:** 🌐 Handle multiple hosts and domains within a single process. Manage and update the dynamic IP addresses for various hosts and domains effortlessly.

- **Simple Configuration:** ⚙️ Easily configure the application using a single command, simplifying the setup process. The configuration file allows you to define multiple hosts and domains, making it flexible and user-friendly.

- **Cross-Platform Compatibility:** 🌍 Designed for multiplatform support, though not tested on all operating systems, the application aims to work seamlessly across various environments. This includes Windows, Linux, and macOS, providing flexibility in choosing your preferred operating system.

- **Automatic IP Detection:** 🎯 Introduces automatic IP detection to streamline the process of obtaining the current IP address, ensuring accuracy in updates.

- **Run as a Service:** 🔄 The application can run as a service, allowing for background execution and automated updates without manual intervention.

- **Single Process Handling:** ⚡ Efficiently manages all the specified features within a single process, simplifying the overall system architecture and resource utilization.


## Installation

1. Clone the repository: `git clone https://github.com/kikelodeon/Namecheap-DDNS-Client.git`
2. Build the solution using Visual Studio or your preferred build tool.

## Usage

```bash
NameCheapDDNSClient C:\\Path\\To\\ConfigFolder
```
### Options

- `--help, -h`: Show this help message.

## Configuration

### Loading Configurations
The application loads configuration files from the specified folder path provided during execution. Ensure your configuration files are placed in the designated `<config_folder_path>`.


### Config File Format
The configuration file is a JSON file with the following structure:
```json
{
  "Host": "your-host",
  "Domain": "your-domain",
  "Password": "your-password",
  "TTL": 30
}
```
Make sure to replace the placeholder values (`your-host`, `your-domain`, `your-password`) with your actual configuration details.

- **Host:** The host or subdomain for which you want to update the dynamic IP address.

  :exclamation: **Tips:**
  - Use `@` when you want to update the root domain.
  - Use `*` when you want to update the dynamic IP address for all subdomains.
  - Specify a specific subdomain when you want to update the IP address for that particular subdomain.


- **Domain:** The main domain associated with the host or subdomain.
- **Password:** The password or authentication token required for updating the dynamic IP address.
- **TTL (Time To Live):** The Time To Live value represents the duration for which the DNS record should be cached.
  
  :exclamation: **Tips:**
  -  The TTL value is **ranged** from 5 to 60 minutes.
  -  Steps of 5 minutes are **clamped** for setting the TTL.


## Version History

- 1.0 (January 2024)

## Author
- Contact: kikelodeon@eggmedia.net

## License

This project is licensed under the MIT License.