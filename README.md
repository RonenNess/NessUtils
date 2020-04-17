# NessUtils

Miscellaneous small C# utils.
Its just random stuff I use in multiple projects and wanted to enjoy the comfort of nuget.

# Install

`Install-Package NessUtils`

Or visit [https://www.nuget.org/packages/NessUtils/](https://www.nuget.org/packages/NessUtils/) for more details.

# APIs

## BinaryBuffer

A simple buffer you can push primitives into.
Used to transfer data efficiently over network.

## BinaryFiles

Open binary files for input / output.

## Configuration

Wraps app.config with slightly easier API.

## ConstantSeededRandom

Random generator with constant seed. Used it when I needed a unified seed-based random generation for both C# and JavaScript.

## EnumTable

A dictionary-like container where key is an enum, with efficiency of an array (enum most be sequencial zero-based numbers).
Also easily serializable into / from XMLs.

## GridUtils

Misc utils for handling a 2d grid of objects.

## MathUtils

Misc math utils.

## SerializableDictionary

An XML-serializable dictionary.

## SparseList

A sparse list that can have holes in it. Should be more efficient that a default list for lots of add/remove actions.

## XmlFiles

Easy XML serialize/deserialize into files.

# License

MIT License.
