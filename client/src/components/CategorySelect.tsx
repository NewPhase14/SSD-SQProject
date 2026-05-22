import { useEffect, useState } from "react";
import { categoryClient } from "../api/clients";
import type { CategoryResponseDto } from "../generated-client";

interface Props {
    value: string | null;
    onChange: (value: string) => void;
}

export default function CategorySelect({ value, onChange }: Props) {
    const [categories, setCategories] = useState<CategoryResponseDto[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        categoryClient
            .getCategories()
            .then(setCategories)
            .finally(() => setLoading(false));
    }, []);

    return (
        <div className="w-full">
            <div className="relative">
                <select
                    value={value ?? ""}
                    onChange={(e) => onChange(e.target.value)}
                    disabled={loading}
                    className="
                        w-full appearance-none
                        border border-gray-200
                        rounded-xl
                        bg-white
                        px-4 py-3
                        text-gray-900
                        focus:outline-none
                        focus:ring-2 focus:ring-blue-500
                        disabled:opacity-60
                        disabled:cursor-not-allowed
                    "
                >
                    <option value="">
                        {loading ? "Loading categories..." : "Select category"}
                    </option>

                    {categories.map((c) => (
                        <option key={c.id} value={c.id}>
                            {c.name}
                        </option>
                    ))}
                </select>

                {/* dropdown icon */}
                <div className="absolute right-3 top-1/2 -translate-y-1/2 pointer-events-none text-gray-400">
                    ▼
                </div>
            </div>
        </div>
    );
}