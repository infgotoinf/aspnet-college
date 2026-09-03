{pkgs ? import <nixpkgs> {}}:
pkgs.mkShell {
  nativeBuildInputs = with pkgs; [
    dotnetCorePackages.dotnet_8.sdk
  ];
}
