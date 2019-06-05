resource "azurerm_virtual_machine" "singlang" {
  name                  = "${local.virtual_machine_name}"
  location              = "${azurerm_resource_group.singlang.location}"
  resource_group_name   = "${azurerm_resource_group.singlang.name}"
  network_interface_ids = ["${azurerm_network_interface.singlang.id}"]
  vm_size               = "Standard_F2s_v2"

  # This means the OS Disk will be deleted when Terraform destroys the Virtual Machine
  # NOTE: This may not be optimal in all cases.
  delete_os_disk_on_termination = true

  storage_image_reference {
    publisher = "credativ"
    offer     = "Debian"
    sku       = "9"
    version   = "latest"
  }

  storage_os_disk {
    name              = "singlang-osdisk"
    caching           = "ReadWrite"
    create_option     = "FromImage"
    managed_disk_type = "Standard_LRS"
  }

  os_profile {
    computer_name  = "${local.virtual_machine_name}"
    admin_username = "${local.admin_username}"
    admin_password = "${local.admin_password}"
  }

  os_profile_linux_config {
    disable_password_authentication = false
  }

  provisioner "remote-exec" {
    connection {
      user     = "${local.admin_username}"
      password = "${local.admin_password}"
    }

    inline = [

    ]
  }
}
resource "azurerm_virtual_machine" "reverse-proxy" {
  name                  = "rproxy-nginx"
  location              = "${azurerm_resource_group.reverse-proxy.location}"
  resource_group_name   = "${azurerm_resource_group.reverse-proxy.name}"
  network_interface_ids = ["${azurerm_network_interface.reverse-proxy.id}"]
  vm_size               = "Standard_B1s"

  # This means the OS Disk will be deleted when Terraform destroys the Virtual Machine
  # NOTE: This may not be optimal in all cases.
  delete_os_disk_on_termination = true

  storage_image_reference {
    publisher = "credativ"
    offer     = "Debian"
    sku       = "9"
    version   = "latest"
  }

  storage_os_disk {
    name              = "proxy-osdisk"
    caching           = "ReadWrite"
    create_option     = "FromImage"
    managed_disk_type = "Standard_LRS"
  }

  os_profile {
    computer_name  = "${local.virtual_machine_name}"
    admin_username = "${local.admin_username}"
    admin_password = "${local.admin_password}"
  }

  os_profile_linux_config {
    disable_password_authentication = false
  }

  provisioner "remote-exec" {
    connection {
      user     = "${local.admin_username}"
      password = "${local.admin_password}"
    }

    inline = [

    ]
  }
}
