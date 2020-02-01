# Changelog

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
