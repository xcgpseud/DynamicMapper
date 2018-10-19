using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mapper.Tools
{
    public class Mapper<TInput, TOutput>
    {
        private readonly Dictionary<string, string> _mappings;
        private readonly object _mappingObject = null;

        private readonly TInput _inputObject;
        private readonly TOutput _outputObject;
        
        private readonly PropertyInfo[] _inputProperties;
        private readonly PropertyInfo[] _mappingObjectProperties;

        private Mapper(TInput inputObject, object mappingObject = null, Dictionary<string, string> mappings = null)
        {
            _inputObject = inputObject;
            _mappingObject = mappingObject;
            _mappings = mappings;
            
            _inputProperties = typeof(TInput).GetProperties();
            if (_mappingObject != null)
            {
                _mappingObjectProperties = _mappingObject.GetType().GetProperties();
            }

            _outputObject = Activator.CreateInstance<TOutput>();
        }

        public TOutput Map()
        {
            foreach (var property in _inputProperties)
            {
                // If we have an extra property with this name, prioritise that
                MapProperty(_inputObject, _outputObject, property.Name,
                    _mappings.ContainsKey(property.Name)
                        ? _mappings[property.Name]
                        : property.Name);
            }

            if (_mappingObject != null)
            {
                foreach (var property in _mappingObjectProperties)
                {
                    if (_mappings.ContainsKey(property.Name))
                    {
                        MapProperty(_mappingObject, _outputObject, property.Name, _mappings[property.Name]);
                    }
                }
            }

            return _outputObject;
        }

        private void MapProperty(
            object inputObject, object outputObject,
            string inputPropertyName, string outputPropertyName = null)
        {
            var outputPropertyCheck = outputPropertyName ?? inputPropertyName;
            
            var inputType = inputObject.GetType();
            var outputType = outputObject.GetType();
            
            var inputProperties = inputType.GetProperties();
            var outputProperties = outputType.GetProperties();
            
            if (inputProperties.Any(prop => prop.Name == inputPropertyName) &&
                outputProperties.Any(prop => prop.Name == outputPropertyCheck))
            {
                outputType.GetProperty(outputPropertyCheck)
                    ?.SetValue(outputObject, inputType.GetProperty(inputPropertyName).GetValue(inputObject));
            }
        }

        public static MapperBuilder<TInput, TOutput> Create()
        {
            return new MapperBuilder<TInput, TOutput>();
        }
        
        public class MapperBuilder<TBuilderInput, TBuilderOutput>
        {
            private readonly Dictionary<string, string> _mappings;
            private object _mappingObject;
            private TBuilderInput _inputObject;
            
            public MapperBuilder()
            {
                _mappings = new Dictionary<string, string>();
            }

            public MapperBuilder<TBuilderInput, TBuilderOutput> WithInputObject(TBuilderInput inputObject)
            {
                _inputObject = inputObject;
                return this;
            }
            
            public MapperBuilder<TBuilderInput, TBuilderOutput> WithMapping(string inputProperty, string outputProperty)
            {
                _mappings.Add(inputProperty, outputProperty);
                return this;
            }

            public MapperBuilder<TBuilderInput, TBuilderOutput> WithMappingObject(object mappingObject)
            {
                _mappingObject = mappingObject;
                return this;
            }
            
            public Mapper<TBuilderInput, TBuilderOutput> Build()
            {
                if (_inputObject == null)
                {
                    throw new Exception("No Input Object Specified");
                }
                
                return new Mapper<TBuilderInput, TBuilderOutput>(_inputObject, _mappingObject, _mappings);
            }
        }
    }
}