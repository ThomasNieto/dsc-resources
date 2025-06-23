// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System.CommandLine;

using Dsc.Resource.CommandLine;
using Dsc.Resource.WindowsService;

var resource = new Resource();
var command = CommandBuilder<Resource, Schema>.Build(resource, SourceGenerationContext.Default);
command.Invoke(args);
