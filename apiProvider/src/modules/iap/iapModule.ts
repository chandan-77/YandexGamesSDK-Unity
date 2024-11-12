export class IAPModule {
    /**
     * Initialize the payments object.
     * @param {string} requestId - The unique request ID.
     */
    static async initializePayments(requestId: string) {
        try {
            const sdk = await window.yandexSDK;
            const payments = await sdk.getPayments({ signed: true });

            // Store the payments object globally for other methods to use
            window.payments = payments;

            // Send success response
            window.SendResponseToUnity(requestId, true, null, null);
            console.log("Payments initialized successfully.");
        } catch (error: any) {
            console.error("Error initializing payments:", error.message || error);
            window.SendResponseToUnity(requestId, false, null, error.message || 'Failed to initialize payments.');
        }
    }

    /**
     * Purchase a product.
     * @param {string} requestId - The unique request ID.
     * @param {string} productId - The product ID to purchase.
     * @param {string} developerPayload - Optional developer payload.
     */
    static async purchaseProduct(requestId: string, productId: string, developerPayload = null) {
        try {
            if (!window.payments) {
                throw new Error('Payments not initialized.');
            }

            const purchase = await window.payments.purchase({
                id: productId,
                developerPayload: developerPayload,
            });

            // Serialize purchase data to JSON
            const jsonData = JSON.stringify(purchase);

            // Send success response with purchase data
            window.SendResponseToUnity(requestId, true, jsonData, null);
            console.log("Product purchased successfully.");
        } catch (error: any) {
            console.error("Error purchasing product:", error.message || error);
            window.SendResponseToUnity(requestId, false, null, error.message || 'Failed to purchase product.');
        }
    }

    /**
     * Get the user's purchases.
     * @param {string} requestId - The unique request ID.
     */
    static async getPurchases(requestId: string) {
        try {
            if (!window.payments) {
                throw new Error('Payments not initialized.');
            }

            const purchases = await window.payments.getPurchases();

            // Serialize purchases data to JSON
            const jsonData = JSON.stringify(purchases);

            // Send success response with purchases data
            window.SendResponseToUnity(requestId, true, jsonData, null);
            console.log("User purchases fetched successfully.");
        } catch (error: any) {
            console.error("Error fetching purchases:", error.message || error);
            window.SendResponseToUnity(requestId, false, null, error.message || 'Failed to fetch purchases.');
        }
    }

    /**
     * Consume a purchase.
     * @param {string} requestId - The unique request ID.
     * @param {string} purchaseToken - The purchase token to consume.
     */
    static async consumePurchase(requestId: string, purchaseToken: string) {
        try {
            if (!window.payments) {
                throw new Error('Payments not initialized.');
            }

            await window.payments.consumePurchase(purchaseToken);

            // Send success response
            window.SendResponseToUnity(requestId, true, null, null);
            console.log("Purchase consumed successfully.");
        } catch (error: any) {
            console.error("Error consuming purchase:", error.message || error);
            window.SendResponseToUnity(requestId, false, null, error.message || 'Failed to consume purchase.');
        }
    }

    /**
     * Get the product catalog.
     * @param {string} requestId - The unique request ID.
     */
    static async getCatalog(requestId: string) {
        try {
            if (!window.payments) {
                throw new Error('Payments not initialized.');
            }

            const products = await window.payments.getCatalog();

            // Serialize products data to JSON
            const jsonData = JSON.stringify(products);

            // Send success response with catalog data
            window.SendResponseToUnity(requestId, true, jsonData, null);
            console.log("Product catalog fetched successfully.", jsonData);
        } catch (error: any) {
            console.error("Error fetching product catalog:", error.message || error);
            window.SendResponseToUnity(requestId, false, null, error.message || 'Failed to fetch product catalog.');
        }
    }
}
