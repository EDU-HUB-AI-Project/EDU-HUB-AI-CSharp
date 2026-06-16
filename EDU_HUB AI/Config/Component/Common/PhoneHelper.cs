namespace EDU_HUB_AI.Config.Component.Common
{
    public static class PhoneHelper
    {
        /// <summary>숫자 11자리(또는 10자리) 연락처를 010-1234-5678 형식으로 변환. 그 외 형식은 원본 반환.</summary>
        public static string Format(string? phone)
        {
            if(string.IsNullOrEmpty(phone))
            {
                return phone ?? "";
            }

            var digits = phone.Replace("-", "").Replace(" ", "");

            if(digits.Length == 11 && digits.All(char.IsDigit))
            {
                return $"{digits.Substring(0, 3)}-{digits.Substring(3, 4)}-{digits.Substring(7, 4)}";
            }
            if(digits.Length == 10 && digits.All(char.IsDigit))
            {
                return $"{digits.Substring(0, 3)}-{digits.Substring(3, 3)}-{digits.Substring(6, 4)}";
            }

            return phone;
        }

    }
}
