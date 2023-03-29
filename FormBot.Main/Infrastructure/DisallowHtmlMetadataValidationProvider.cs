using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FormBot.Main.Infrastructure
{
    public class DisallowHtmlMetadataValidationProvider : DataAnnotationsModelValidatorProvider
    {
        protected override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata,
       ControllerContext context, IEnumerable<Attribute> attributes)
        {
            if (attributes == null)
                return base.GetValidators(metadata, context, null);

            if (string.IsNullOrEmpty(metadata.PropertyName))
                return base.GetValidators(metadata, context, attributes);

            //DisallowHtml should not be added if a property allows html input
            var isHtmlInput = attributes.OfType<AllowHtmlAttribute>().Any();
            if (isHtmlInput)
                return base.GetValidators(metadata, context, attributes);

            attributes = new List<Attribute>(attributes) { new DisallowHtmlAttribute() };
            return base.GetValidators(metadata, context, attributes);
        }
    }
}