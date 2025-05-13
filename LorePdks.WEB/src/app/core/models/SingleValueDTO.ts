/**
 * Generic olarak farklı tiplerle kullanılabilecek tek değer taşıyan DTO sınıfı.
 * C# tarafındaki SingleValueDTO<T> yapısının TypeScript karşılığıdır.
 */
export class SingleValueDTO<T> {
    /**
     * Taşınan değer
     */
    public Value: T;

    /**
     * Sınıf constructor'ı
     * @param value Taşınacak değer (opsiyonel)
     */
    constructor(value?: T) {
        this.Value = value;
    }
}
