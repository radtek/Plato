﻿using Microsoft.Extensions.Configuration;

namespace PlatoCore.Yaml
{

    public class YamlConfigurationSource : FileConfigurationSource
    {

        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            FileProvider = FileProvider ?? builder.GetFileProvider();
            return new YamlConfigurationProvider(this);
        }

    }

}
