using EDU_HUB_AI.Config;
using EDU_HUB_AI.exception;
using EDU_HUB_AI.Model;
using System.Text;
using System.Text.Json;
using System.Diagnostics;

namespace EDU_HUB_AI.Util
{
    public class ApiClient
    {
        private readonly AppConfig? _appConfig = new AppConfig();
        private readonly HttpClient? _httpClient = new HttpClient();
        private string baseUrl;
        private string apiKeys;
        // 최대 시도 회수 지정
        private readonly int _maxAttempt = 5;

        public ApiClient()
        {
            _appConfig = JsonSerializer.Deserialize<AppConfig>(File.ReadAllText("config.json"));
            baseUrl = _appConfig?.ApiSettings?.BaseUrl;
            apiKeys = _appConfig?.ApiSettings?.ApiKeys;
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.Add("X-Api-Secret", apiKeys); 
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json")); // 서버한테 JSON으로 응답해달라고 요청
        }

        public async Task<ApiResponse<T>> Get<T>(string url) // Task: js의 promise와 같은 역할
        {
            // 시도 회수 지정
            int attempt = 0;
            while (true) 
            {
                try
                {   
                    attempt++;
                    HttpResponseMessage response = await _httpClient.GetAsync(url); // ResponseEntity
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<T>>(json);
                    // status가 400 이상이면 예외로 이동
                    if(result?.Status >= 400) throw new ApiException(result.Status, result.Message);
                    return result;
                } 
                catch (ApiException ex) when(ex.Status >= 400 && ex.Status < 500)
                {
                    // 클라이언트에서 발생하는 예외는 재실행 x
                    throw;
                }
                // 서버 및 기타 예외는 재실행 최대 5회까지
                catch (ApiException ex) when (ex.Status == 500)
                {
                    Debug.WriteLine($"재시도 횟수: {attempt}회", ex.Message);
                    // 최대 회수 채우면 종료
                    if (attempt >= _maxAttempt) throw;
                    // 2초 후 재실행
                    await Task.Delay(2000);                  
                }
                catch (Exception ex) // 기존에 정의된 status 이외의 예외
                {
                    Debug.WriteLine($"재시도 횟수: {attempt}회", ex.Message);
                    if (attempt >= _maxAttempt) throw;
                    await Task.Delay(2000);
                } 
            }
        }

        public async Task<ApiResponse<T>> Post<T>(string? url, Object? body)
        {
            int attempt = 0;
            while (true)
            {
                try
                {
                    attempt++;
                    var json = JsonSerializer.Serialize(body);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");  // 클라이언트에서 보내는 데이터
                    HttpResponseMessage response = await _httpClient.PostAsync(url, content);

                    var res = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<T>>(res);
                    if (result?.Status >= 400) throw new ApiException(result.Status, result.Message);
                    return result;
                }
                catch (ApiException ex) when (ex.Status >= 400 && ex.Status < 500)
                {
                    throw;
                }
                catch (ApiException ex) when (ex.Status == 500)
                {
                    Debug.WriteLine($"재시도 횟수: {attempt}회", ex.Message);
                    if (attempt >= _maxAttempt) throw;
                    await Task.Delay(2000);
                }
                catch (Exception ex) // 기존에 정의된 status 이외의 예외
                {
                    Debug.WriteLine($"재시도 횟수: {attempt}회", ex.Message);
                    if (attempt >= _maxAttempt) throw;
                    await Task.Delay(2000);
                }
            }

        }
        public async Task<ApiResponse<T>> Put<T>(string? url, Object? body)
        {
            int attempt = 0;
            while (true)
            {
                try {
                    attempt++;
                    var json = JsonSerializer.Serialize(body);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await _httpClient.PutAsync(url, content);

                    var res = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<T>>(res);
                    if (result?.Status >= 400) throw new ApiException(result.Status, result.Message);
                    return result;
                }
                catch (ApiException ex) when (ex.Status >= 400 && ex.Status < 500)
                {
                    throw;
                }
                catch (ApiException ex) when (ex.Status == 500)
                {
                    Debug.WriteLine($"재시도 횟수: {attempt}회", ex.Message);
                    if (attempt >= _maxAttempt) throw;
                    await Task.Delay(2000);
                }
                catch (Exception ex) // 기존에 정의된 status 이외의 예외
                {
                    Debug.WriteLine($"재시도 횟수: {attempt}회", ex.Message);
                    if (attempt >= _maxAttempt) throw;
                    await Task.Delay(2000);
                }
            }
                
        }
        public async Task<ApiResponse<T>> Patch<T>(string? url, Object? body)
        {
            int attempt = 0;
            while (true) 
            {
                try
                {
                    attempt++;
                    var json = JsonSerializer.Serialize(body);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await _httpClient.PatchAsync(url, content);

                    var res = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<T>>(res);
                    if (result?.Status >= 400) throw new ApiException(result.Status, result.Message);
                    return result;
                }
                catch (ApiException ex) when (ex.Status >= 400 && ex.Status < 500)
                {
                    throw;
                }
                catch (ApiException ex) when (ex.Status == 500)
                {
                    Debug.WriteLine($"재시도 횟수: {attempt}회", ex.Message);
                    if (attempt >= _maxAttempt) throw;
                    await Task.Delay(2000);
                }
                catch (Exception ex) // 기존에 정의된 status 이외의 예외
                {
                    Debug.WriteLine($"재시도 횟수: {attempt}회", ex.Message);
                    if (attempt >= _maxAttempt) throw;
                    await Task.Delay(2000);
                }
            }
            
        }

        public async Task<ApiResponse<T>> Delete<T>(string? url)
        {
            int attempt = 0;
            while (true)
            {
                try
                {
                    attempt++;
                    HttpResponseMessage response = await _httpClient.DeleteAsync(url);
                    var res = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<T>>(res);
                    if (result?.Status >= 400) throw new ApiException(result.Status, result.Message);
                    return result;
                }
                catch (ApiException ex) when (ex.Status >= 400 && ex.Status < 500)
                {
                    throw;
                }
                catch (ApiException ex) when (ex.Status == 500)
                {
                    Debug.WriteLine($"재시도 횟수: {attempt}회", ex.Message);
                    if (attempt >= _maxAttempt) throw;
                    await Task.Delay(2000);
                }
                catch (Exception ex) // 기존에 정의된 status 이외의 예외
                {
                    Debug.WriteLine($"재시도 횟수: {attempt}회", ex.Message);
                    if (attempt >= _maxAttempt) throw;
                    await Task.Delay(2000);
                }
            }
        }
    }
}
