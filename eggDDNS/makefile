GROUP_NAME = egg
APP_NAME = eggDDNS
INSTALL_DIR = /opt/$(APP_NAME)
APPDATA_DIR = /etc/$(APP_NAME)
SERVICE_DIR = /etc/systemd/system
# Folders/files to be moved.
SERVICE_FILE = $(INSTALL_DIR)/$(APP_NAME).service
APPDATA_FOLDER = $(INSTALL_DIR)/AppData

install:
	# Ask for root password at the beginning
	@echo "Starting installation..."
	@sudo echo "Root password required for installation"

	@if sudo id -u $(APP_NAME) > /dev/null 2>&1; then \
		echo "User $(APP_NAME) already exists. Skipping user creation."; \
	else \
		sudo getent group $(GROUP_NAME) || sudo groupadd $(GROUP_NAME); \
		sudo getent group root || sudo groupadd root; \
		sudo useradd -m -G $(GROUP_NAME),root $(APP_NAME); \
		sudo passwd $(APP_NAME); \
		echo "$(APP_NAME) ALL=(ALL:ALL) ALL" | sudo tee -a /etc/sudoers; \
	fi

	@echo "Creating installation directory..."; 
	sudo mkdir -p $(INSTALL_DIR)

	@echo "Copying files..."; 
	@sudo cp -r $(CURDIR)/* $(INSTALL_DIR)/

	@echo "Moving service file to $(SERVICE_DIR)...";
	@sudo mv $(SERVICE_FILE) $(SERVICE_DIR)/

	@echo "Creating application data directory..."; 
	sudo mkdir -p $(APPDATA_DIR)

	@echo "Moving example configs to $(APPDATA_DIR)...";
	@sudo mv $(APPDATA_FOLDER) $(APPDATA_DIR)/

	@echo "Creating symlink...";
	@sudo ln -s $(INSTALL_DIR)/$(APP_NAME) /usr/local/bin/$(APP_NAME)

	@echo "Enabling the service...";
	@sudo systemctl enable $(APP_NAME)
	@echo "Installation success!";
	
uninstall:
	# Ask for root password at the beginning
	@echo "Starting uninstallation..."
	@sudo echo "Root password required for uninstallation"

	@if sudo id -u $(APP_NAME) > /dev/null 2>&1; then \
		echo "Disabling and stopping the service...";
		@sudo systemctl stop $(APP_NAME); \
		@sudo systemctl disable $(APP_NAME); \
		echo "Removing service file from $(SERVICE_DIR)...";
		@sudo rm -f $(SERVICE_DIR)/$(APP_NAME).service; \
		sudo userdel -r $(APP_NAME); \
		sudo groupdel $(GROUP_NAME); \
		echo "User $(APP_NAME) and associated files removed."; \
		echo "Removing files..."; \
		sudo rm -rf $(INSTALL_DIR); \
		echo "Removing symlink..."; \
		sudo rm -f /usr/local/bin/$(APP_NAME); \		
	else \
		echo "User $(APP_NAME) not found. Skipping uninstallation."; \
	fi

