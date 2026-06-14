using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDU_HUB_AI.Util
{
    // 프로퍼티에만 사용 가능한 커스텀 어트리뷰트 정의
    // AttributeTargets.Property → 클래스나 메서드에 붙이면 컴파일 에러 
    [AttributeUsage(AttributeTargets.Property)]
    public class ExcelColumnAttribute : Attribute // Attribute 상속 → 어트리뷰트로 사용 가능
    {
        // 엑셀 헤더명을 저장하는 프로퍼티 (읽기 전용)
        public string Name { get; }
        // 생성자 → [ExcelColumn("필드명")] 에서 "필드명"이 name으로 들어옴
        public ExcelColumnAttribute(string name) => Name = name;
    }
}
