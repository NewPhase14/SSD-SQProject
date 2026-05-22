import { useEffect, useState } from "react";
import { listingClient } from "../api/clients";
import type { ListingResponseDto } from "../generated-client";

export default function ListingDetails({
                                           listingId,
                                           onBack,
                                       }: {
    listingId: string;
    onBack: () => void;
}) {
    const [listing, setListing] = useState<ListingResponseDto | null>(null);
    const [activeImage, setActiveImage] = useState(0);

    useEffect(() => {
        listingClient.getListingById(listingId).then(setListing);
    }, [listingId]);

    if (!listing) {
        return <div className="p-6 text-gray-500">Loading...</div>;
    }

    const images = listing.imageUrls ?? [];

    return (
        <div className="max-w-5xl mx-auto px-4 py-10">
            <button
                onClick={onBack}
                className="text-blue-600 hover:underline mb-6"
            >
                ← Back
            </button>

            <div className="grid md:grid-cols-2 gap-10">
                {/* IMAGE SECTION */}
                <div>
                    <div className="rounded-2xl overflow-hidden bg-gray-100 h-[420px]">
                        {images.length > 0 ? (
                            <img
                                src={images[activeImage]}
                                className="w-full h-full object-cover"
                                alt="listing"
                            />
                        ) : (
                            <div className="h-full flex items-center justify-center text-gray-400">
                                No images
                            </div>
                        )}
                    </div>

                    <div className="flex gap-2 mt-3 overflow-x-auto">
                        {images.map((img, i) => (
                            <img
                                key={i}
                                src={img}
                                onClick={() => setActiveImage(i)}
                                className={`h-16 w-16 object-cover rounded-lg cursor-pointer border-2 ${
                                    i === activeImage
                                        ? "border-blue-500"
                                        : "border-gray-200"
                                }`}
                                alt={`thumb-${i}`}
                            />
                        ))}
                    </div>
                </div>

                {/* INFO */}
                <div>
                    <h1 className="text-3xl font-bold text-gray-900">
                        {listing.title}
                    </h1>

                    <p className="text-gray-600 mt-3">
                        {listing.description}
                    </p>

                    <div className="mt-6 text-3xl font-bold text-green-600">
                        {listing.price} DKK
                    </div>

                    <div className="mt-2 text-sm text-gray-500">
                        Condition: {listing.condition}
                    </div>

                    <button className="mt-8 w-full bg-blue-600 text-white py-3 rounded-xl hover:bg-blue-700 transition font-semibold">
                        Message Seller
                    </button>
                </div>
            </div>
        </div>
    );
}