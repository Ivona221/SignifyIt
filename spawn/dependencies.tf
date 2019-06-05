locals {
  virtual_machine_name = "mitre-test-vm"
  admin_username       = "mitre"
  admin_password       = ""
}

resource "azurerm_resource_group" "singlang" {
  name     = "man-test-research"
  location = "westeurope"
}

resource "azurerm_virtual_network" "singlang" {
  name                = "singlang-network"
  address_space       = ["10.0.0.0/16"]
  location            = "${azurerm_resource_group.singlang.location}"
  resource_group_name = "${azurerm_resource_group.singlang.name}"
}

resource "azurerm_subnet" "singlang" {
  name                 = "internal"
  resource_group_name  = "${azurerm_resource_group.singlang.name}"
  virtual_network_name = "${azurerm_virtual_network.singlang.name}"
  address_prefix       = "10.0.2.0/24"
}

resource "azurerm_public_ip" "singlang" {
  name                = "singlang-publicip"
  resource_group_name = "${azurerm_resource_group.singlang.name}"
  location            = "${azurerm_resource_group.singlang.location}"
  allocation_method   = "Static"
}

resource "azurerm_network_interface" "singlang" {
  name                = "singlang-nic"
  location            = "${azurerm_resource_group.singlang.location}"
  resource_group_name = "${azurerm_resource_group.singlang.name}"

  ip_configuration {
    name                          = "configuration"
    subnet_id                     = "${azurerm_subnet.singlang.id}"
    private_ip_address_allocation = "Dynamic"
    public_ip_address_id          = "${azurerm_public_ip.singlang.id}"
  }
}
