using System.Collections.Generic;
using Abp.Runtime.Validation;

namespace MyTrainingV1231AngularDemo.Editions.Dto
{
	//Mapped in CustomDtoMapper
	public class FeatureInputTypeDto
	{
		public string Name { get; set; }

		public IDictionary<string, object> Attributes { get; set; }

		public IValueValidator Validator { get; set; }

		public LocalizableComboboxItemSourceDto ItemSource { get; set; }
	}
}