# Changelog

## [1.3.0] - 2020-03-18

Flatten namespaces

### Changes

* Flatten namespaces for using simply

## [1.2.0] - 2020-02-07

### Changes

* Follow CAFU v3.1 rules

## [1.1.3] - 2020-02-03

### Fixes

* Fully support IL2CPP

## [1.1.2] - 2020-02-03

### Fixes

* Add `link.xml` to preserve all types

## [1.1.1] - 2020-02-01

### Changes

* Make SignalHandler class and constructor public so that you can write code to avoid code stripping by IL2CPP

## [1.1.0] - 2020-02-01

* Add features

### Features

* #8 Implement caching signals
* #7 Implement termination signal subscription

### Changes

* Move installation method to `SignalHandlerInstaller<TSignal>`

### Fixes

* Experimental: Fixed IL2CPP related exceptions

## [1.0.2] - 2020-01-31

### Fixes

* Experimental: Fixed IL2CPP related exceptions

## [1.0.1] - 2020-01-31

### Fixes

* Experimental: Fixed IL2CPP related exceptions

## [1.0.0] - 2020-01-29

* Initial version

### Features

* Provide `ISignalPublisher<T>` and `ISignalReceiver<T>` to handle signal uses `Zenject.SignalBus`
