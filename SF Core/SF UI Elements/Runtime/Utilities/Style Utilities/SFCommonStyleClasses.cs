namespace SF.UIElements.Utilities
{
    /// <summary>
    /// The USS style class names are from the CommonUss.uss style asset. 
    /// This class holds a list of commonly used USS style class names.
    /// It is intended to just help not have to memorize the names of certain USS classes.
    /// It also has certain combo of commonly used together USS classes names like a two column row.
    /// Read the remark in the file to see a nice clean way to not have to type out the full class name when using them.
    /// </summary>
    /// <remarks>
    /// For those not wanting to type out SFCommonStyleClasses everytime they want to use one of the class names.
    /// Do this in your using namespace declarations and than you can just type out the variable name byitself.
    /// 
    /// using static SF.UIElements.Utilities.SFCommonStyleClasses;
    /// </remarks>
    public static class SFCommonStyleClasses
    {
        /* Some values have to use static readonly since const values have to be a compiled time constant.
         * For instance you can't have a compile time constant for a new array declaration.
        */

        public static readonly string TwoColumn = "two-column";
        public static readonly string[] TwoColumnRow = new string[] { "sf-row","two-column" };
    }
}
