#!/usr/bin/env bash

touch "$HOME/.dotnet/$(dotnet --version).dotnetFirstUseSentinel"

pwsh \
    -NoLogo \
    -NoProfile \
    -NonInteractive \
    -ExecutionPolicy "Bypass" \
    -File "./build.ps1" %*
