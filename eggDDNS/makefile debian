APP_NAME = eggDDNS
GROUP_NAME = egg
INSTALL_DIR = /opt/$(APP_NAME)
APPDATA_DIR = /etc/$(APP_NAME)
SERVICE_DIR = /etc/systemd/system
SERVICE_FILE = $(INSTALL_DIR)/$(APP_NAME).service
APPDATA_FOLDER = $(INSTALL_DIR)/AppData
ADDUSER := adduser
ADDGROUP := addgroup

check_sudo:
	@if [ "$$(id -u)" -ne 0 ]; then \
		echo "Please run 'make install' as root."; \
		exit 1; \
	fi

install: check_sudo
	# Ask for root password at the beginning
	@echo "Starting installation..."
	@echo "Root privileges may be required for installation"

	@id -u $(APP_NAME) ; \
	if [ $$? -eq 0 ]; then \
		echo "User $(APP_NAME) already exists. Skipping user creation."; \
	else \
		$(ADDGROUP) $(GROUP_NAME); \
		$(ADDUSER) --allow-bad-names $(APP_NAME); \
		usermod -aG $(GROUP_NAME) $(APP_NAME); \
		usermod -aG root $(APP_NAME); \
		echo "$(APP_NAME) ALL=(ALL:ALL) ALL" | tee -a /etc/sudoers; \
	fi

	@echo "Creating installation directory..."
	@mkdir -p $(INSTALL_DIR)

	@echo "Copying files..."
	@cp -r $(CURDIR)/* $(INSTALL_DIR)/

	@echo "Copiying service file to $(SERVICE_DIR)..."
	@cp -r $(SERVICE_FILE) $(SERVICE_DIR)/

	@echo "Creating application data directory..."
	@mkdir -p $(APPDATA_FOLDER)

	@echo "Copiying example configs to $(APPDATA_DIR)..."
	@cp -r $(APPDATA_FOLDER) $(APPDATA_DIR)/

	@echo "Creating symlink..."
	@if [ ! -e "/usr/local/bin/$(APP_NAME)" ]; then \
	    ln -s $(INSTALL_DIR)/$(APP_NAME) /usr/local/bin/$(APP_NAME); \
	    echo "Symlink created!"; \
	else \
	    echo "Symlink already exists."; \
	fi

	@echo "Giving permisions..."
	@chown root:root /usr/local/bin/$(APP_NAME)
	@chown $(APP_NAME):$(APP_NAME) /usr/local/bin/$(APP_NAME)
	@chmod +x /usr/local/bin/$(APP_NAME)

	@systemctl is-enabled $(APP_NAME) > /dev/null 2>&1; \
	if [ $$? -eq 0 ]; then \
	    echo "Service already enabled!"; \
	else \
    	systemctl enable $(APP_NAME); \
    	    echo "Service enabled."; \
	fi
	@echo "Installation success!"

uninstall:
	@echo "Starting uninstallation..."
	@echo "Root privileges may be required for uninstallation"

	@echo "Stopping and disabling the service..."
	@systemctl stop $(APP_NAME)
	@systemctl disable $(APP_NAME)
	@systemctl daemon-reload

	@echo "Removing files..."
	@rm -rf $(INSTALL_DIR)
	@rm -f /usr/local/bin/$(APP_NAME)

	@echo "Removing service file..."
	@rm -f $(SERVICE_DIR)/$(APP_NAME).service

	@echo "Removing application data directory..."
	@rm -rf $(APPDATA_DIR)

	@echo "Removing user and group..."
	@deluser $(APP_NAME)
	@delgroup $(GROUP_NAME)

	@echo "Reloading systemd daemon..."
	@systemctl daemon-reload

	@echo "Uninstallation completed."