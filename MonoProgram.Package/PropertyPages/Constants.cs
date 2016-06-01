namespace MonoProgram.Package.PropertyPages
{
    public static class Constants
    {
        public const int SW_SHOW = 5;
        public const int SW_SHOWNORMAL = 1;
        public const int SW_HIDE = 0;

        /// <summary>
        /// The values in the pages have changed, so the state of the
        /// Apply button should be updated.
        /// </summary>
        public const int PROPPAGESTATUS_DIRTY = 0x1;

        /// <summary>
        /// Now is an appropriate time to apply changes.
        /// </summary>
        public const int PROPPAGESTATUS_VALIDATE = 0x2;
    }
}