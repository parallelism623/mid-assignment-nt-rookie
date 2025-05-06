
using MIDASM.Contract.SharedKernel;
using System.Collections;

namespace MISARM.API.Tests.DataTestSources;

public class ApiBaseControllerTestDatas : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { Result<string>.Success("Test200")};
        yield return new object[] { Result<string>.Success("Test204", 204) };
        yield return new object[] { Result<string>.Failure(400) };
        yield return new object[] { Result<string>.Failure(401) };
        yield return new object[] { Result<string>.Failure(403) };
        yield return new object[] { Result<string>.Failure(404) };
    }


    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}


public class ApiBaseControllerTestFileDatas : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { new byte[] { 0x01, 0x02, 0x03, 0x04 }, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", ""};
        yield return new object[] { new byte[] { 0x01, 0x02, 0x03, 0x04 }, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "test.xlsx"};
        yield return new object[] { new byte[] { 0x01, 0x02, 0x03, 0x04 }, "application/pdf", ""};
        yield return new object[] { new byte[] { 0x01, 0x02, 0x03, 0x04 }, "application/pdf", "text.pdf"};
    }


    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}