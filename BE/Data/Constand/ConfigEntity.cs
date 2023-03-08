namespace BE.Data.Constand
{
	public static class ConfigEntity
	{
		public static class Rules
		{
			public const int MAX_LENGTH_TITLE = 300;
			public const string TABLE_NAME = "Rules";
		}
        public static class Notifications
        {
            public const int MAX_LENGTH_TITLE = 300;
            public const string TABLE_NAME = "Notification";
			public const int MAX_LENGTH_MESSAGE = 500;
        }

        public static class BlockingWeb
        {
            public const string TABLE_NAME = "BlockingWeb";
        }
    }
}
