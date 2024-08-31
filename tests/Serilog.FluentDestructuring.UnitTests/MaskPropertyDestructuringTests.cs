using FluentAssertions;
using Serilog.Events;
using Serilog.FluentDestructuring.UnitTests.Infrastructure;
using Serilog.FluentDestructuring.UnitTests.Models;

namespace Serilog.FluentDestructuring.UnitTests;

public class MaskPropertyDestructuringTests
{
    [Fact]
    public void String_Property_Should_Be_Masked_With_Default_Masking_Processor_When_This_Is_Configured()
    {
        var obj = new MaskPropertyModel
        {
            String = "SampleString",
        };
        var expected = new ScalarValue("**********");
        
        var evt = DelegateSink.Execute<TestFluentDestructuringPolicy>(obj);
        var sv = (StructureValue)evt.Properties[DelegateSink.ParamName];
        var properties = sv.Properties.ToDictionary(e => e.Name, e => e.Value);

        properties[nameof(MaskPropertyModel.String)].Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void String_Property_Should_Be_Masked_With_Default_Masking_Processor_Length_Preservation_When_This_Is_Configured()
    {
        var obj = new MaskPropertyModel
        {
            StringPreserveLength = "PreserveLengthString",
        };
        var expected = new ScalarValue("********************");
        
        var evt = DelegateSink.Execute<TestFluentDestructuringPolicy>(obj);
        var sv = (StructureValue)evt.Properties[DelegateSink.ParamName];
        var properties = sv.Properties.ToDictionary(e => e.Name, e => e.Value);

        properties[nameof(MaskPropertyModel.StringPreserveLength)].Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void String_Property_Should_Be_Masked_With_Default_Masking_Processor_Length_Preservation_And_Custom_Mask_Character_When_This_Is_Configured()
    {
        var obj = new MaskPropertyModel
        {
            StringWithPreserveLengthAndCustomMaskCharacter = "string",
        };
        var expected = new ScalarValue("$$$$$$");
        
        var evt = DelegateSink.Execute<TestFluentDestructuringPolicy>(obj);
        var sv = (StructureValue)evt.Properties[DelegateSink.ParamName];
        var properties = sv.Properties.ToDictionary(e => e.Name, e => e.Value);

        properties[nameof(MaskPropertyModel.StringWithPreserveLengthAndCustomMaskCharacter)].Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void String_Property_Should_Be_Masked_With_Default_Masking_Processor_Customizing_Mask_Character_When_This_Is_Configured()
    {
        var obj = new MaskPropertyModel
        {
            StringWithCustomMaskCharacter = "strongPassword",
        };
        var expected = new ScalarValue("$$$$$$$$$$");
        
        var evt = DelegateSink.Execute<TestFluentDestructuringPolicy>(obj);
        var sv = (StructureValue)evt.Properties[DelegateSink.ParamName];
        var properties = sv.Properties.ToDictionary(e => e.Name, e => e.Value);

        properties[nameof(MaskPropertyModel.StringWithCustomMaskCharacter)].Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void String_Property_Should_Be_Masked_With_Default_Masking_Processor_Customizing_Mask_Character_And_Mask_Length_When_This_Is_Configured()
    {
        var obj = new MaskPropertyModel
        {
            StringWithCustomMaskCharacterAndCustomMaskLength = "anyValue",
        };
        var expected = new ScalarValue("@@@@@@@@@@@@@@@");
        
        var evt = DelegateSink.Execute<TestFluentDestructuringPolicy>(obj);
        var sv = (StructureValue)evt.Properties[DelegateSink.ParamName];
        var properties = sv.Properties.ToDictionary(e => e.Name, e => e.Value);

        properties[nameof(MaskPropertyModel.StringWithCustomMaskCharacterAndCustomMaskLength)].Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void String_Property_Should_Be_Masked_With_Custom_Masking_Processor_When_This_Is_Configured()
    {
        var obj = new MaskPropertyModel
        {
            StringCustomMaskingProcessor = "CustomProcessorString",
        };
        var expected = new ScalarValue(TestMaskingProcessor.MaskedValue);
        
        var evt = DelegateSink.Execute<TestFluentDestructuringPolicy>(obj);
        var sv = (StructureValue)evt.Properties[DelegateSink.ParamName];
        var properties = sv.Properties.ToDictionary(e => e.Name, e => e.Value);

        properties[nameof(MaskPropertyModel.StringCustomMaskingProcessor)].Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void String_Collection_Property_Should_Be_Masked_With_Default_Masking_Processor_When_This_Is_Configured()
    {
        var obj = new MaskPropertyModel
        {
            StringIEnumerable = new List<string>
            {
                "First",
                "Second",
                "Third"
            },
        };
        var expected = new SequenceValue([ new ScalarValue("**********"), new ScalarValue("**********"), new ScalarValue("**********") ]);
        
        var evt = DelegateSink.Execute<TestFluentDestructuringPolicy>(obj);
        var sv = (StructureValue)evt.Properties[DelegateSink.ParamName];
        var properties = sv.Properties.ToDictionary(e => e.Name, e => e.Value);

        properties[nameof(MaskPropertyModel.StringIEnumerable)].Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void Not_String_Property_Should_Not_Be_Masked_With_Default_Masking_Processor_When_This_Is_Configured()
    {
        var obj = new MaskPropertyModel
        {
            NotString = 45
        };
        var expected = new ScalarValue(obj.NotString);
        
        var evt = DelegateSink.Execute<TestFluentDestructuringPolicy>(obj);
        var sv = (StructureValue)evt.Properties[DelegateSink.ParamName];
        var properties = sv.Properties.ToDictionary(e => e.Name, e => e.Value);

        properties[nameof(MaskPropertyModel.NotString)].Should().BeEquivalentTo(expected);
    }
}