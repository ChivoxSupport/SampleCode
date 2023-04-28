using System;
namespace ChivoxAIEngine
{
public class RetValue
{
public readonly int ErrID = ChivoxErrorCode.OK;
public readonly string Error = ChivoxErrorText.OK;
public RetValue()
{
}
public RetValue(Exception e)
{
ErrID = ChivoxErrorCode.INNER_AGN_CALL_ERR;
Error = e.Message;
}
public RetValue(ChivoxException e)
{
ErrID = e.ErrorCode;
Error = e.Message;
}
}
}
