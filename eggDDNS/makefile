APP_NAME = EggAgent
INSTALL_DIR = /opt/$(APP_NAME)
SERVICE_DIR = /etc/systemd/system

install:
	# Ask for root password at the beginning
	@echo "Starting installation..."
	@sudo echo "Root password required for installation"

	@if sudo id -u $(APP_NAME) > /dev/null 2>&1; then \
		echo "User $(APP_NAME) already exists. Skipping user creation."; \
	else \
		sudo getent group egg || sudo groupadd egg; \
		sudo getent group root || sudo groupadd root; \
		sudo useradd -m -G egg,root $(APP_NAME); \
		sudo passwd $(APP_NAME); \
		echo "EggAgent ALL=(ALL:ALL) ALL" | sudo tee -a /etc/sudoers; \
	fi

	@if sudo id -u $(APP_NAME) > /dev/null 2>&1; then \
		sudo -u $(APP_NAME) -H bash -c '\
			echo "User: $$USER, Home: $$HOME"; \
			if [ "`uname`" = "Darwin" ]; then \
				brew install rsnapshot; \
			elif command -v apt-get > /dev/null 2>&1; then \
				sudo apt-get update && sudo apt-get install -y rsnapshot; \
			elif command -v yum > /dev/null 2>&1; then \
				sudo yum makecache && sudo yum install -y rsnapshot; \
			elif command -v zypper > /dev/null 2>&1; then \
				sudo zypper refresh && sudo zypper install -y rsnapshot; \
			elif command -v pacman > /dev/null 2>&1; then \
				sudo pacman -Syu --noconfirm && sudo pacman -S --noconfirm rsnapshot; \
			elif command -v dnf > /dev/null 2>&1; then \
				sudo dnf makecache && sudo dnf install -y rsnapshot; \
			elif command -v apk > /dev/null 2>&1; then \
				sudo apk update && sudo apk add rsnapshot; \
			else \
				echo "Unable to determine the package manager. Please install rsnapshot manually."; \
				exit 1; \
			fi'; \
	else \
		echo "User $(APP_NAME) not found. Aborting installation."; \
		exit 1; \
	fi

	@echo "Creating installation directory..."; 
	sudo mkdir -p $(INSTALL_DIR)

	@echo "Copying files..."; 
	@sudo cp -r $(CURDIR)/* $(INSTALL_DIR)/

	@echo "Creating symlink..."
	@sudo ln -s $(INSTALL_DIR)/EggAgent /usr/local/bin/EggAgent

uninstall:
	# Ask for root password at the beginning
	@echo "Starting uninstallation..."
	@sudo echo "Root password required for uninstallation"

	@if sudo id -u $(APP_NAME) > /dev/null 2>&1; then \
		sudo userdel -r $(APP_NAME); \
		sudo groupdel egg; \
		echo "User $(APP_NAME) and associated files removed."; \
		echo "Removing files..."; \
		sudo rm -rf $(INSTALL_DIR); \
		echo "Removing symlink..."; \
		sudo rm -f /usr/local/bin/EggAgent; \
		if command -v apt-get > /dev/null 2>&1; then \
			sudo apt-get remove -y rsnapshot; \
		elif command -v yum > /dev/null 2>&1; then \
			sudo yum remove -y rsnapshot; \
		elif command -v zypper > /dev/null 2>&1; then \
			sudo zypper remove -y rsnapshot; \
		elif command -v pacman > /dev/null 2>&1; then \
			sudo pacman -Rns --noconfirm rsnapshot; \
		elif command -v dnf > /dev/null 2>&1; then \
			sudo dnf remove -y rsnapshot; \
		elif command -v apk > /dev/null 2>&1; then \
			sudo apk del rsnapshot; \
		else \
			echo "Unable to determine the package manager. Please uninstall rsnapshot manually."; \
		fi; \
	else \
		echo "User $(APP_NAME) not found. Skipping uninstallation."; \
	fi

	