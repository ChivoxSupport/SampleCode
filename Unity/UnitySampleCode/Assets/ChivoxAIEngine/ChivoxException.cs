using System;
namespace ChivoxAIEngine
{
public class ChivoxException : ApplicationException
{
public readonly int ErrorCode;
public ChivoxException() : base(ChivoxErrorText.OK)
{
ErrorCode = ChivoxErrorCode.OK;
}
public ChivoxException(int errorCode, string message) : base(message)
{
ErrorCode = errorCode;
}
public ChivoxException(int errorCode, string message, Exception innerException) : base(message, innerException)
{
ErrorCode = errorCode;
}
}
public static class ChivoxErrorCode
{
public const int OK = 0;
public const int ARGUMENT_NULL = 900002;
public const int MAKE_CFG_TEXT_ERR = 900003;
public const int MAKE_START_TEXT_ERR = 900004;
public const int ENGINE_CALL_ORDER_ERR = 900005;
public const int ENGINE_DESTROYED = 900006;
public const int NO_RECORD_PERMISSION = 900007;
public const int RECORDER_IN_USE = 900008;
public const int RECORDER_START_ERR = 900009;
public const int RECORDER_EXCEPTION = 900010;
public const int INNER_AGN_IN_USE = 900011;
public const int INNER_AGN_CALL_ERR = 900012;
public const int INNER_AGN_DELETED = 900013;
public const int RESULT_NOT_VALID_JSON = 900014;
}
internal static class ChivoxErrorText
{
public const string OK = "OK";
public const string ARGUMENT_NULL = "ARGUMENT_NULL";
public const string MAKE_CFG_TEXT_ERR = "MAKE_CFG_TEXT_ERR";
public const string MAKE_START_TEXT_ERR = "MAKE_START_TEXT_ERR";
public const string ENGINE_CALL_ORDER_ERR = "ENGINE_CALL_ORDER_ERR";
public const string ENGINE_DESTROYED = "ENGINE_DESTROYED";
public const string NO_RECORD_PERMISSION = "NO_RECORD_PERMISSION";
public const string RECORDER_IN_USE = "RECORDER_IN_USE";
public const string RECORDER_START_ERR = "RECORDER_START_ERR";
public const string RECORDER_EXCEPTION = "RECORDER_EXCEPTION";
public const string INNER_AGN_IN_USE = "INNER_AGN_IN_USE";
public const string INNER_AGN_CALL_ERR = "INNER_AGN_CALL_ERR";
public const string INNER_AGN_DELETED = "INNER_AGN_DELETED";
public const string RESULT_NOT_VALID_JSON = "RESULT_NOT_VALID_JSON";
}
}
