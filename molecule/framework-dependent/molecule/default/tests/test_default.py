import os

import testinfra.utils.ansible_runner

testinfra_hosts = testinfra.utils.ansible_runner.AnsibleRunner(
    os.environ['MOLECULE_INVENTORY_FILE']).get_hosts('all')


def test_package_is_runnable(host):
    host.run_test("/usr/share/framework-dependent-app/framework-dependent-app")


def test_package_symlink_is_runnable(host):
    host.run_test("/usr/local/bin/framework-dependent-app")


def test_readme_is_deployed(host):
    assert host.file("/etc/dotnet-packaging/README.md").exists
