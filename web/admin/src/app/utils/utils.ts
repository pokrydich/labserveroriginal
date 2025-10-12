

export class Utils {
    public static deepCopy<T>(obj: T): T {
        return JSON.parse(JSON.stringify(obj));
    }
}

/**
 * Helper collection type that tracks updates (intended updates source: SignalR)
 */
export class UpdatableMap<TElem> {
    private innerMap: { [key: string]: TElem } = {}
    private keyGetter: (elem: TElem) => string;
    private elemComparer?: (first: TElem, second: TElem) => number;

    public getElements(): TElem[] {
        const values = Object.values(this.innerMap);
        return this.elemComparer !== null
            ? values.sort(this.elemComparer)
            : values;
    }

    public getElement(id: string): TElem | undefined {
        return this.innerMap[id];
    }

    constructor(keyGetter: (elem: TElem) => string,
                elemComparer: ((first: TElem, second: TElem) => number) | undefined = undefined) {
        this.keyGetter = keyGetter;
        this.elemComparer = elemComparer;
    }

    public setElems(elems: TElem[]) {
        this.innerMap = {};
        for (let elem of elems) {
            this.innerMap[this.keyGetter(elem)] = elem;
        }
    }

    public addElems(elems: TElem[]) {
        for (let elem of elems) {
            this.innerMap[this.keyGetter(elem)] = elem;
        }
    }

    public onUpdate(update: TElem) {
        const key = this.keyGetter(update);
        this.innerMap[key] = update;
    }
}